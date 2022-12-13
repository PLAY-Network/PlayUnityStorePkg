using System;
using NUnit.Framework;
using RGN.Extensions;
using RGN.Impl.Firebase.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RGN.Model;
using RGN.Modules.Store;
using RGN.Utility;
using UnityEngine;
using UnityEngine.TestTools;

namespace RGN.Store.Tests.Runtime
{
    [TestFixture]
    public class StoreTests
    {
        [OneTimeSetUp]
        public async void OneTimeSetup()
        {
            var applicationStore = ApplicationStore.I;
            RGNCoreBuilder.AddModule(new StoreModule());
            var appOptions = new AppOptions()
            {
                ApiKey = applicationStore.RGNMasterApiKey,
                AppId = applicationStore.RGNMasterAppID,
                ProjectId = applicationStore.RGNMasterProjectId
            };

            await RGNCoreBuilder.Build(
                new RGN.Impl.Firebase.Dependencies(
                    appOptions,
                    applicationStore.RGNStorageURL),
                appOptions,
                applicationStore.RGNStorageURL,
                applicationStore.RGNAppId);

            if (applicationStore.usingEmulator)
            {
                RGNCore rgnCore = (RGNCore)RGNCoreBuilder.I;
                var firestore = rgnCore.readyMasterFirestore;
                string firestoreHost = applicationStore.emulatorServerIp + applicationStore.firestorePort;
                bool firestoreSslEnabled = false;
                firestore.UserEmulator(firestoreHost, firestoreSslEnabled);
                rgnCore.readyMasterFunction.UseFunctionsEmulator(applicationStore.emulatorServerIp + applicationStore.functionsPort);
            }
        }
        
        // TODO: cover buy functions under tests

        [UnityTest]
        public IEnumerator AddVirtualItemShopOffer_ChecksCreatedOffer()
        {
            var task = AddStoreOffer();
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;
            
            yield return DeleteStoreOffer(result.id);
            
            Assert.NotNull(result, "Store offer didn't added to db");
        }

        [UnityTest]
        public IEnumerator GetByTags_ReturnsArrayOfOffers()
        {
            var tagsToFind = new[] { "testItemTag1", "testItemTag2" };
            
            var addStoreOfferTask = AddStoreOffer();
            yield return addStoreOfferTask.AsIEnumeratorReturnNull();
            var addStoreOfferResult = addStoreOfferTask.Result;
            
            var getStoreOffersByTagsTask = RGNCoreBuilder.I.GetModule<StoreModule>()
                .GetByTags(tagsToFind);
            yield return getStoreOffersByTagsTask.AsIEnumeratorReturnNull();
            var getStoreOffersByTagsResult = getStoreOffersByTagsTask.Result;
            
            yield return DeleteStoreOffer(addStoreOfferResult.id);
            
            Assert.IsNotEmpty(getStoreOffersByTagsResult.offers);

            var areOfferTagsCorrect =
                tagsToFind.Any(x => getStoreOffersByTagsResult.offers[0].tags.Contains(x));

            Assert.True(areOfferTagsCorrect, "Retrieved store offer doesn't contains any requested tag");
            
            var noDuplicates = getStoreOffersByTagsResult.offers
                .GroupBy(x => x.id).Any(g => g.Count() <= 1);
            
            Assert.True(noDuplicates, "Request returns duplicated store offers");
        }

        [UnityTest]
        public IEnumerator GetByAppIds_ReturnsArrayOfOffers()
        {
            var appIdsToFind = new[] { "io.getready.rgntest", "anotherAppId" };
            
            var addStoreOfferTask = AddStoreOffer();
            yield return addStoreOfferTask.AsIEnumeratorReturnNull();
            var addStoreOfferResult = addStoreOfferTask.Result;
            
            var getStoreOffersByAppIdsTask = RGNCoreBuilder.I.GetModule<StoreModule>()
                .GetByAppIds(appIdsToFind, 2);
            yield return getStoreOffersByAppIdsTask.AsIEnumeratorReturnNull();
            var getStoreOffersByAppIdsResult = getStoreOffersByAppIdsTask.Result;
            
            yield return DeleteStoreOffer(addStoreOfferResult.id);
            
            Assert.IsNotEmpty(getStoreOffersByAppIdsResult.offers);

            var areOfferAppIdsCorrect =
                appIdsToFind.Any(x => getStoreOffersByAppIdsResult.offers[0].appIds.Contains(x));

            Assert.True(areOfferAppIdsCorrect, "Retrieved store offer doesn't contains any requested appId");
            
            var noDuplicates = getStoreOffersByAppIdsResult.offers
                .GroupBy(x => x.id).Any(g => g.Count() <= 1);
            
            Assert.True(noDuplicates, "Request returns duplicated store offers");
        }
        
        [UnityTest]
        public IEnumerator GetByIds_ReturnsArrayOfOffers()
        {
            string[] idsToFind = new string[2];
            
            var addStoreOfferTask = AddStoreOffer();
            yield return addStoreOfferTask.AsIEnumeratorReturnNull();
            var addStoreOfferResult = addStoreOfferTask.Result;
            
            idsToFind[0] = addStoreOfferResult.id;
            idsToFind[1] = addStoreOfferResult.id;
            
            var getStoreOffersByIdsTask = RGNCoreBuilder.I.GetModule<StoreModule>()
                .GetByIds(idsToFind);
            yield return getStoreOffersByIdsTask.AsIEnumeratorReturnNull();
            var getStoreOffersByIdsResult = getStoreOffersByIdsTask.Result;
            
            yield return DeleteStoreOffer(addStoreOfferResult.id);
            
            Assert.IsNotEmpty(getStoreOffersByIdsResult.offers);

            var areOfferIdsCorrect = getStoreOffersByIdsResult.offers.All(x => idsToFind.Contains(x.id));

            Assert.True(areOfferIdsCorrect, "Retrieved store offer doesn't much requested any id");
            
            var noDuplicates = getStoreOffersByIdsResult.offers
                .GroupBy(x => x.id).Any(g => g.Count() <= 1);
            
            Assert.True(noDuplicates, "Request returns duplicated store offers");
        }

        [UnityTest]
        public IEnumerator GetTags_ReturnsArrayOfOfferTags()
        {
            var expectedTags = new[]
            {
                "testItemTag1", "testItemTag2"
            };
            
            var addStoreOfferTask = AddStoreOffer();
            yield return addStoreOfferTask.AsIEnumeratorReturnNull();
            var addStoreOfferResult = addStoreOfferTask.Result;
            
            var getStoreOfferTagsTask = RGNCoreBuilder.I.GetModule<StoreModule>()
                .GetTags(addStoreOfferResult.id);
            yield return getStoreOfferTagsTask.AsIEnumeratorReturnNull();
            var getStoreOfferTagsResult = getStoreOfferTagsTask.Result;
            
            yield return DeleteStoreOffer(addStoreOfferResult.id);
            
            var tagsAreEqual = expectedTags.Length == getStoreOfferTagsResult.tags.Length;
            if (tagsAreEqual)
            {
                for (var i = 0; i < expectedTags.Length; i++)
                {
                    if (expectedTags[i].Equals(getStoreOfferTagsResult.tags[i]))
                    {
                        continue;
                    }
                    tagsAreEqual = false;
                    break;
                }
            }
            
            Assert.True(tagsAreEqual, "Expected tags doesn't equals to actual");
        }

        [UnityTest]
        public IEnumerator SetTags_ChecksSetTags()
        {
            var newTags = new []
            {
                "tag1",
                "tag2",
                "tag3",
            };
            
            var addStoreOfferTask = AddStoreOffer();
            yield return addStoreOfferTask.AsIEnumeratorReturnNull();
            var addStoreOfferResult = addStoreOfferTask.Result;

            var setTagsTask = RGNCoreBuilder.I.GetModule<StoreModule>()
                .SetTags(addStoreOfferResult.id, newTags);
            yield return setTagsTask.AsIEnumeratorReturnNull();
            
            var getStoreOfferTask = GetStoreOffer(addStoreOfferResult.id);
            yield return getStoreOfferTask.AsIEnumeratorReturnNull();
            var getStoreOfferResult = getStoreOfferTask.Result;
            
            yield return DeleteStoreOffer(addStoreOfferResult.id);

            var tagsAreEqual = newTags.Length == getStoreOfferResult.tags.Length;
            if (tagsAreEqual)
            {
                for (var i = 0; i < newTags.Length; i++)
                {
                    if (newTags[i].Equals(getStoreOfferResult.tags[i]))
                    {
                        continue;
                    }
                    tagsAreEqual = false;
                    break;
                }
            }
            Assert.True(tagsAreEqual, "Tags field didn't set properly");
        }

        [UnityTest]
        public IEnumerator SetName_ChecksSetName()
        {
            var newName = "New name for test";
            
            var addStoreOfferTask = AddStoreOffer();
            yield return addStoreOfferTask.AsIEnumeratorReturnNull();
            var addStoreOfferResult = addStoreOfferTask.Result;

            var setNameTask = RGNCoreBuilder.I.GetModule<StoreModule>()
                .SetName(addStoreOfferResult.id, newName);
            yield return setNameTask.AsIEnumeratorReturnNull();
            
            var getStoreOfferTask = GetStoreOffer(addStoreOfferResult.id);
            yield return getStoreOfferTask.AsIEnumeratorReturnNull();
            var getStoreOfferResult = getStoreOfferTask.Result;
            
            yield return DeleteStoreOffer(addStoreOfferResult.id);
            
            Assert.AreEqual(newName, getStoreOfferResult.name, "Name field didn't set properly");
        }

        [UnityTest]
        public IEnumerator SetDescription_ChecksSetName()
        {
            var newDescription = "New description for test";
            
            var addStoreOfferTask = AddStoreOffer();
            yield return addStoreOfferTask.AsIEnumeratorReturnNull();
            var addStoreOfferResult = addStoreOfferTask.Result;

            var setDescriptionTask = RGNCoreBuilder.I.GetModule<StoreModule>()
                .SetDescription(addStoreOfferResult.id, newDescription);
            yield return setDescriptionTask.AsIEnumeratorReturnNull();
            
            var getStoreOfferTask = GetStoreOffer(addStoreOfferResult.id);
            yield return getStoreOfferTask.AsIEnumeratorReturnNull();
            var getStoreOfferResult = getStoreOfferTask.Result;
            
            yield return DeleteStoreOffer(addStoreOfferResult.id);
            
            Assert.AreEqual(newDescription, getStoreOfferResult.description, "Description field didn't set properly");
        }

        [UnityTest]
        public IEnumerator SetPrices_ChecksSetPrices()
        {
            var newPrices = new []
            {
                new RGNStoreOfferPrice("itemId1", "currency1", 1),
                new RGNStoreOfferPrice("itemId1", "currency2", 1),
            };
            
            var addStoreOfferTask = AddStoreOffer();
            yield return addStoreOfferTask.AsIEnumeratorReturnNull();
            var addStoreOfferResult = addStoreOfferTask.Result;

            var setPricesTask = RGNCoreBuilder.I.GetModule<StoreModule>()
                .SetPrices(addStoreOfferResult.id, newPrices);
            yield return setPricesTask.AsIEnumeratorReturnNull();
            
            var getStoreOfferTask = GetStoreOffer(addStoreOfferResult.id);
            yield return getStoreOfferTask.AsIEnumeratorReturnNull();
            var getStoreOfferResult = getStoreOfferTask.Result;
            
            yield return DeleteStoreOffer(addStoreOfferResult.id);

            var pricesAreEqual = newPrices.Length == getStoreOfferResult.prices.Length;
            if (pricesAreEqual)
            {
                for (var i = 0; i < newPrices.Length; i++)
                {
                    if (newPrices[i].Equals(getStoreOfferResult.prices[i]))
                    {
                        continue;
                    }
                    pricesAreEqual = false;
                    break;
                }
            }
            Assert.True(pricesAreEqual, "Prices field didn't set properly");
        }

        [UnityTest]
        public IEnumerator SetTime_ChecksSetTime()
        {
            var newTime = new RGNStoreOfferTime(0, 1000, 100, 50);
            
            var addStoreOfferTask = AddStoreOffer();
            yield return addStoreOfferTask.AsIEnumeratorReturnNull();
            var addStoreOfferResult = addStoreOfferTask.Result;

            var setTimeTask = RGNCoreBuilder.I.GetModule<StoreModule>()
                .SetTime(addStoreOfferResult.id, newTime);
            yield return setTimeTask.AsIEnumeratorReturnNull();
            
            var getStoreOfferTask = GetStoreOffer(addStoreOfferResult.id);
            yield return getStoreOfferTask.AsIEnumeratorReturnNull();
            var getStoreOfferResult = getStoreOfferTask.Result;
            
            yield return DeleteStoreOffer(addStoreOfferResult.id);
            
            Assert.AreEqual(newTime, getStoreOfferResult.time, "Time field didn't set properly");
        }

        [UnityTest]
        public IEnumerator SetImageUrl_ChecksSetImageUrl()
        {
            var newImageUrl = "New image url for test";
            
            var addStoreOfferTask = AddStoreOffer();
            yield return addStoreOfferTask.AsIEnumeratorReturnNull();
            var addStoreOfferResult = addStoreOfferTask.Result;

            var setImageUrlTask = RGNCoreBuilder.I.GetModule<StoreModule>()
                .SetImageUrl(addStoreOfferResult.id, newImageUrl);
            yield return setImageUrlTask.AsIEnumeratorReturnNull();
            
            var getStoreOfferTask = GetStoreOffer(addStoreOfferResult.id);
            yield return getStoreOfferTask.AsIEnumeratorReturnNull();
            var getStoreOfferResult = getStoreOfferTask.Result;
            
            yield return DeleteStoreOffer(addStoreOfferResult.id);
            
            Assert.AreEqual(newImageUrl, getStoreOfferResult.imageUrl, "ImageUrl field didn't set properly");
        }

        [UnityTest]
        public IEnumerator DeleteStoreOffer_ConfirmDeleting()
        {
            var addStoreOfferTask = AddStoreOffer();
            yield return addStoreOfferTask.AsIEnumeratorReturnNull();
            var addStoreOfferResult = addStoreOfferTask.Result;

            var deleteStoreOfferTask = DeleteStoreOffer(addStoreOfferResult.id);
            yield return deleteStoreOfferTask.AsIEnumeratorReturnNull();

            var getStoreOfferTask = GetStoreOffer(addStoreOfferResult.id);
            yield return getStoreOfferTask.AsIEnumeratorReturnNull();
            var getStoreOfferResult = getStoreOfferTask.Result;
            
            Assert.IsNull(getStoreOfferResult, "Store offer didn't completely deleted");
        }

        private async Task<RGNStoreOffer> AddStoreOffer()
        {
            var task = RGNCoreBuilder.I.GetModule<StoreModule>().AddVirtualItemsShopOffer(
                new [] { "io.getready.rgntest", "anotherAppId" },
                new [] { "itemId1" },
                "testItemName",
                "testItemDesc",
                new [] { "testItemTag1", "testItemTag2" },
                1);
            var result = await task;
            return result;
        }

        private async Task<RGNStoreOffer> GetStoreOffer(string offerId)
        {
            var task = RGNCoreBuilder.I.GetModule<StoreModule>().GetByIds(new [] { offerId });
            var result = await task;
            return result.offers.Length > 0 ? result.offers[0] : null;
        }

        private async Task DeleteStoreOffer(string offerId)
        {
            await RGNCoreBuilder.I.GetModule<StoreModule>().DeleteStoreOffer(offerId);
        }
    }
}