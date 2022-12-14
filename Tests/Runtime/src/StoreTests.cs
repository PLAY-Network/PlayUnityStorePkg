using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using RGN.Dependencies;
using RGN.Dependencies.Core;
using RGN.Extensions;
using RGN.Impl.Firebase.Core;
using RGN.Modules.EmailSignIn;
using RGN.Tests;
using RGN.Modules.Store;
using UnityEngine;
using UnityEngine.TestTools;

namespace RGN.Store.Tests.Runtime
{
    [TestFixture]
    public class StoreTests : BaseTests
    {
        #region Configuration

        protected override IDependencies Dependencies => new Impl.Firebase.Dependencies(new AppOptions
        {
            ApiKey = AppOptions.ApiKey,
            AppId = AppOptions.AppId,
            ProjectId = AppOptions.ProjectId
        }, ApplicationStore.I.RGNStorageURL);
        protected override IAppOptions AppOptions => new AppOptions
        {
            ApiKey = ApplicationStore.I.RGNMasterApiKey,
            AppId = ApplicationStore.I.RGNAppId,
            ProjectId = ApplicationStore.I.RGNMasterProjectId,
        };
        protected override string StorageUrl => ApplicationStore.I.RGNStorageURL;
        protected override bool UseEmulator => ApplicationStore.I.usingEmulator;
        protected override string EmulatorServerIp => ApplicationStore.I.emulatorServerIp;
        protected override string EmulatorFunctionsPort => ApplicationStore.I.functionsPort;
        protected override string EmulatorFirestorePort => ApplicationStore.I.firestorePort;
        protected override List<IRGNModule> Modules => new List<IRGNModule>()
        {
            emailSignInModule,
            new StoreModule()
        };
        protected override EmailLogInDelegate EmailLogIn => emailSignInModule.OnSignInWithEmail;
        protected override EmailLogOutDelegate EmailLogOut => emailSignInModule.SignOutFromEmail;
        
        private readonly EmailSignInModule emailSignInModule = new EmailSignInModule();
        
        [UnitySetUp]
        public IEnumerator UnitySetUp()
        {
            yield return SetUp();
        }

        #endregion

        #region Tests
        
        [UnityTest]
        public IEnumerator BuyVirtualItems()
        {
            throw new Exception();
        }
        
        [UnityTest]
        public IEnumerator BuyVirtualItem()
        {
            throw new Exception();
        }
        
        [UnityTest]
        public IEnumerator AddVirtualItemShopOffer_ChecksCreatedOffer()
        {
            yield return LoginAsAdminTester();
            
            var task = AddStoreOffer();
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;
            
            yield return DeleteStoreOffer(result.id);
            
            Assert.NotNull(result, "Store offer didn't added to db");
        }
        
        [UnityTest]
        public IEnumerator AddVirtualItemShopOffer_CanBeCalledOnlyWithAdminRights()
        {
            yield return LoginAsNormalTester();
            
            var task = AddStoreOffer();
            yield return task.AsIEnumeratorReturnNullDontThrow();
        
            if (!task.IsFaulted)
            {
                yield return DeleteStoreOffer(task.Result.id);
            }
        
            Assert.True(task.IsFaulted, "Store offer anyway was added to db even with no admin rights");
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
            yield return LoginAsAdminTester();
            
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
            yield return LoginAsAdminTester();
            
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
            yield return LoginAsAdminTester();
            
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
            yield return LoginAsAdminTester();
            
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
            yield return LoginAsAdminTester();
            
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
            yield return LoginAsAdminTester();
            
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
            yield return LoginAsAdminTester();
            
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
        
        [UnityTest]
        public IEnumerator SetProperties_ReturnsPropertiesThatWasSet()
        {
            yield return LoginAsAdminTester();
            
            var storeOfferId = "bZqexd5Ily1HzhqUZcrd";
            var propertiesToSet = "{}";
        
            var task = RGNCoreBuilder.I.GetModule<StoreModule>().SetProperties(storeOfferId, propertiesToSet);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;
        
            Assert.NotNull(result, "The result is null");
            Assert.AreEqual(propertiesToSet, result);
            Debug.Log(result);
        }
        
        [UnityTest]
        public IEnumerator GetProperties_ReturnsPropertiesThatWasSetBeforeInDB()
        {
            var storeOfferId = "bZqexd5Ily1HzhqUZcrd";
            var expectedProperties = "{}";
        
            var task = RGNCoreBuilder.I.GetModule<StoreModule>().GetProperties(storeOfferId);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;
        
            Assert.NotNull(result, "The result is null");
            Assert.AreEqual(expectedProperties, result);
            Debug.Log(result);
        }
        
        #endregion
        
        #region Common methods
        
        private async Task<RGNStoreOffer> AddStoreOffer()
        {
            var task = RGNCoreBuilder.I.GetModule<StoreModule>().AddVirtualItemsShopOffer(
                new [] { "io.getready.rgntest", "anotherAppId" },
                new [] { "itemId1" },
                "testItemName",
                "testItemDesc",
                new [] { "testItemTag1", "testItemTag2" });
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
        
        #endregion
    }
}