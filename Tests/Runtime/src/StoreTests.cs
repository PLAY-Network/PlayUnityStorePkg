using NUnit.Framework;
using RGN.Modules.Store;
using RGN.Modules.VirtualItems;
using RGN.Tests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RGN.Extensions;
using UnityEngine;
using UnityEngine.TestTools;

namespace RGN.Store.Tests.Runtime
{
    public class StoreTests : BaseTests
    {
        protected override List<IRGNModule> Modules { get; } = new List<IRGNModule>()
        {
            new StoreModule()
        };

        #region Tests

        private static List<List<string>> _testCurrencies1 = new List<List<string>> {
            new List<string> { "test-coin" },
            new List<string> { "test-coin", "test2-coin" } };

        [UnityTest]
        public IEnumerator BuyVirtualItems_WithoutOffer([ValueSource(nameof(_testCurrencies1))] List<string> currencies)
        {
            yield return LoginAsNormalTester();
            
            var itemIds = new List<string> { "ed589211-466b-4d87-9c94-e6ba03a10765" }; // specially created item for tests

            var task = StoreModule.I.BuyVirtualItemsAsync(itemIds, currencies);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.IsNotEmpty(result.itemIds);
            Assert.IsNotEmpty(result.itemIds[0]);
        }

        [UnityTest]
        public IEnumerator BuyVirtualItems_WithOffer([ValueSource(nameof(_testCurrencies1))] List<string> currencies)
        {
            yield return LoginAsNormalTester();
            
            var itemIds = new List<string> { "ed589211-466b-4d87-9c94-e6ba03a10765" }; // specially created item for tests
            var offerId = "NEIoJ3uobAWr4CrFNrW6"; // specially created offer for tests

            var task = StoreModule.I.BuyVirtualItemsAsync(itemIds, currencies, offerId);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.IsNotEmpty(result.offerId);
            Assert.IsNotEmpty(result.itemIds);
            Assert.IsNotEmpty(result.itemIds[0]);
        }

        [UnityTest]
        public IEnumerator BuyVirtualItems_CheckUserCurrencies()
        {
            yield return LoginAsNormalTester();
            
            var itemIds = new List<string> { "ed589211-466b-4d87-9c94-e6ba03a10765" }; // specially created item for tests
            var offerId = "NEIoJ3uobAWr4CrFNrW6"; // specially created offer for tests
            var currencies = new List<string> { "currency-that-no-one-else-have" };

            var task = StoreModule.I.BuyVirtualItemsAsync(itemIds, currencies, offerId);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.IsEmpty(result.itemIds, "User could purchase item even without currency");
        }
        
        [UnityTest]
        public IEnumerator BuyStoreOffer([ValueSource(nameof(_testCurrencies1))] List<string> currencies)
        {
            yield return LoginAsNormalTester();
            
            var offerId = "NEIoJ3uobAWr4CrFNrW6"; // specially created offer for tests

            var task = StoreModule.I.BuyStoreOfferAsync(offerId, currencies);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.IsNotEmpty(result.offerId);
            Assert.IsNotEmpty(result.itemIds);
        }
        
        [UnityTest]
        public IEnumerator BuyVirtualItems_WithoutOffer_NoPrice()
        {
            yield return LoginAsNormalTester();
            
            var itemIds = new List<string> { "7dNN81eLr8XsMxFlgsbH" }; // specially created item for tests

            var task = StoreModule.I.BuyVirtualItemsAsync(itemIds);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.IsNotEmpty(result.itemIds);
            Assert.IsNotEmpty(result.itemIds[0]);
        }
        
        [UnityTest]
        public IEnumerator BuyVirtualItems_WithOffer_NoPrice()
        {
            yield return LoginAsNormalTester();
            
            var itemIds = new List<string> { "7dNN81eLr8XsMxFlgsbH" }; // specially created item for tests
            var offerId = "G0mEAb1LK1aPn9DGsKUo"; // specially created offer for tests

            var task = StoreModule.I.BuyVirtualItemsAsync(itemIds, null, offerId);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.IsNotEmpty(result.offerId);
            Assert.IsNotEmpty(result.itemIds);
            Assert.IsNotEmpty(result.itemIds[0]);
        }
        
        [UnityTest]
        public IEnumerator BuyVirtualItems_WithoutOffer_Simplified()
        {
            yield return LoginAsNormalTester();
            
            var itemIds = new List<string> { "fcb8b866-2ec6-46c1-9dde-3740fa5ebbba" }; // specially created item for tests

            var task = StoreModule.I.BuyVirtualItemsAsync(itemIds);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.IsNotEmpty(result.itemIds);
            Assert.IsNotEmpty(result.itemIds[0]);
        }

        [UnityTest]
        public IEnumerator BuyVirtualItems_WithOffer_Simplified()
        {
            yield return LoginAsNormalTester();
            
            var itemIds = new List<string> { "fcb8b866-2ec6-46c1-9dde-3740fa5ebbba" }; // specially created item for tests
            var offerId = "H8piC6rc9CYTLNUIGcJ4"; // specially created offer for tests

            var task = StoreModule.I.BuyVirtualItemsAsync(itemIds, null, offerId);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.IsNotEmpty(result.offerId);
            Assert.IsNotEmpty(result.itemIds);
            Assert.IsNotEmpty(result.itemIds[0]);
        }
        
        [UnityTest]
        public IEnumerator BuyStoreOffer_Simplified()
        {
            yield return LoginAsNormalTester();
            
            var offerId = "H8piC6rc9CYTLNUIGcJ4"; // specially created offer for tests

            var task = StoreModule.I.BuyStoreOfferAsync(offerId);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.IsNotEmpty(result.offerId);
            Assert.IsNotEmpty(result.itemIds);
        }

        [UnityTest]
        public IEnumerator AddVirtualItemShopOffer_ChecksCreatedOffer()
        {
            yield return LoginAsAdminTester();

            var task = AddStoreOfferAsync();
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            yield return DeleteStoreOfferAsync(result.id);

            Assert.NotNull(result, "Store offer didn't added to db");
        }

        [UnityTest]
        public IEnumerator AddVirtualItemShopOffer_CanBeCalledOnlyWithAdminRights()
        {
            yield return LoginAsNormalTester();

            var task = AddStoreOfferAsync();
            yield return task.AsIEnumeratorReturnNullDontThrow();

            if (!task.IsFaulted)
            {
                yield return DeleteStoreOfferAsync(task.Result.id);
            }

            // TODO: uncomment when the user roles are implemented
            //Assert.True(task.IsFaulted, "Store offer anyway was added to db even with no admin rights");
        }

        [UnityTest]
        public IEnumerator ImportStoreOffersFromCSV_ReturnArrayOfImportedOffers()
        {
            yield return LoginAsAdminTester();

            var csvContent = "name,description,appIds,tags,imageUrl,time,properties,itemIds,prices\nitem1,item1," +
                             "\"[\"\"app1\"\",\"\"app2\"\"]\",\"[\"\"tag1\"\",\"\"tag2\"\"]\",https://site.com/image.png," +
                             "\"{\"\"start\"\":null,\"\"end\"\":null,\"\"intervalDuration\"\":null," +
                             "\"\"intervalDelay\"\":null}\",[],\"[\"\"item1Id\"\"]\",[]";
            var csvDelimiter = ",";

            var task = StoreModule.I.ImportStoreOffersFromCSVAsync(csvContent, csvDelimiter);
            yield return task.AsIEnumeratorReturnNullDontThrow();
            var result = task.Result;
            
            foreach (StoreOffer storeOffer in result)
            {
                yield return DeleteStoreOfferAsync(storeOffer.id);
            }

            Assert.IsNotEmpty(result);
        }

        [UnityTest]
        public IEnumerator GetByTags_ReturnsArrayOfOffers()
        {
            yield return LoginAsAdminTester();
            
            var tagsToFind = new List<string> { "testItemTag1", "testItemTag2" };

            var addStoreOfferTask = AddStoreOfferAsync();
            yield return addStoreOfferTask.AsIEnumeratorReturnNull();
            var addStoreOfferResult = addStoreOfferTask.Result;

            var getStoreOffersByTagsTask = StoreModule.I.GetByTagsAsync(tagsToFind);
            yield return getStoreOffersByTagsTask.AsIEnumeratorReturnNull();
            var getStoreOffersByTagsResult = getStoreOffersByTagsTask.Result;

            yield return DeleteStoreOfferAsync(addStoreOfferResult.id);

            Assert.IsNotEmpty(getStoreOffersByTagsResult);

            var areOfferTagsCorrect =
                tagsToFind.Any(x => getStoreOffersByTagsResult[0].tags.Contains(x));

            Assert.True(areOfferTagsCorrect, "Retrieved store offer doesn't contains any requested tag");

            var noDuplicates = getStoreOffersByTagsResult
                .GroupBy(x => x.id)
                .Any(g => g.Count() <= 1);

            Assert.True(noDuplicates, "Request returns duplicated store offers");
        }

        [UnityTest]
        public IEnumerator GetByTimestamp_ReturnsArrayOfOffers()
        {
            yield return LoginAsAdminTester();

            var randomAppId = Guid.NewGuid().ToString();
            
            var newTime = TimeInfo.CreateWithStartAndEndTime(0, 1000);
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            
            var addStoreOfferTask = AddStoreOfferAsync(new List<string> { randomAppId });
            yield return addStoreOfferTask.AsIEnumeratorReturnNull();
            var addStoreOfferResult = addStoreOfferTask.Result;

            var setTimeTask = StoreModule.I
                .SetTimeAsync(addStoreOfferResult.id, newTime);
            yield return setTimeTask.AsIEnumeratorReturnNull();
            
            var getStoreOffersTimestampTask = StoreModule.I
                .GetByTimestampAsync(randomAppId, dateTime);
            yield return getStoreOffersTimestampTask.AsIEnumeratorReturnNull();
            var getStoreOffersByTimestampResult = getStoreOffersTimestampTask.Result;

            yield return DeleteStoreOfferAsync(addStoreOfferResult.id);

            Assert.IsNotEmpty(getStoreOffersByTimestampResult);
            Assert.AreEqual(addStoreOfferResult.id, getStoreOffersByTimestampResult[0].id, "Wrong retrieved offers");
        }

        [UnityTest]
        public IEnumerator GetByAppIds_ReturnsArrayOfOffers()
        {
            yield return LoginAsAdminTester();
            
            var appIdsToFind = new List<string> { RGNCoreBuilder.I.AppIDForRequests, "anotherAppId" };

            var addStoreOfferTask = AddStoreOfferAsync();
            yield return addStoreOfferTask.AsIEnumeratorReturnNull();
            var addStoreOfferResult = addStoreOfferTask.Result;

            var getStoreOffersByAppIdsTask = StoreModule.I
                .GetByAppIdsAsync(appIdsToFind, 2);
            yield return getStoreOffersByAppIdsTask.AsIEnumeratorReturnNull();
            var getStoreOffersByAppIdsResult = getStoreOffersByAppIdsTask.Result;

            yield return DeleteStoreOfferAsync(addStoreOfferResult.id);

            Assert.IsNotEmpty(getStoreOffersByAppIdsResult);

            var areOfferAppIdsCorrect =
                appIdsToFind.Any(x => getStoreOffersByAppIdsResult[0].appIds.Contains(x));

            Assert.True(areOfferAppIdsCorrect, "Retrieved store offer doesn't contains any requested appId");

            var noDuplicates = getStoreOffersByAppIdsResult
                .GroupBy(x => x.id).Any(g => g.Count() <= 1);

            Assert.True(noDuplicates, "Request returns duplicated store offers");
        }

        [UnityTest]
        public IEnumerator GetByCurrentAppId_ReturnsArrayOfOffers()
        {
            yield return LoginAsAdminTester();
            
            var addStoreOfferTask = AddStoreOfferAsync();
            yield return addStoreOfferTask.AsIEnumeratorReturnNull();
            var addStoreOfferResult = addStoreOfferTask.Result;

            var getStoreOffersForCurrentAppIdTask = StoreModule.I.GetForCurrentAppAsync(3);
            yield return getStoreOffersForCurrentAppIdTask.AsIEnumeratorReturnNull();
            var getStoreOffersForCurrentAppIdResult = getStoreOffersForCurrentAppIdTask.Result;

            yield return DeleteStoreOfferAsync(addStoreOfferResult.id);

            Assert.IsNotEmpty(getStoreOffersForCurrentAppIdResult);

            var areOfferAppIdsCorrect =
                getStoreOffersForCurrentAppIdResult.All(x => x.appIds.Contains(RGNCore.I.AppIDForRequests));

            Assert.True(areOfferAppIdsCorrect, "Retrieved store offer doesn't contains current app id");

            var noDuplicates = getStoreOffersForCurrentAppIdResult
                .GroupBy(x => x.id).Any(g => g.Count() <= 1);

            Assert.True(noDuplicates, "Request returns duplicated store offers");
        }

        [UnityTest]
        public IEnumerator GetByAppIds_PaginationTest()
        {
            yield return LoginAsAdminTester();
            
            var appIdToFind = new List<string> { RGNCoreBuilder.I.AppIDForRequests + ".ordering" };
            var countOffers = 7; // must be odd

            var createdOffers = new List<string>();
            for (int i = 0; i < countOffers; i++)
            {
                var addStoreOfferTask = AddStoreOfferAsync(appIdToFind);
                yield return addStoreOfferTask.AsIEnumeratorReturnNull();
                var addStoreOfferResult = addStoreOfferTask.Result;
                createdOffers.Add(addStoreOfferResult.id);
            }
            createdOffers.Sort(StringComparer.Ordinal);

            var firstPartitionTask = StoreModule.I
                .GetByAppIdsAsync(appIdToFind, 3, createdOffers[0], true);
            yield return firstPartitionTask.AsIEnumeratorReturnNull();
            var firstPartitionResult = firstPartitionTask.Result;
            
            var secondPartitionTask = StoreModule.I
                .GetByAppIdsAsync(appIdToFind, 3, createdOffers[3], true);
            yield return secondPartitionTask.AsIEnumeratorReturnNull();
            var secondPartitionResult = secondPartitionTask.Result;

            for (int i = 0; i < countOffers; i++)
            {
                yield return DeleteStoreOfferAsync(createdOffers[i]);
            }

            string[] expectedIds = createdOffers.Skip(1).ToArray();
            string[] firstPartitionIds = firstPartitionResult.Select(x => x.id).ToArray();
            string[] secondPartitionIds = secondPartitionResult.Select(x => x.id).ToArray();
            string[] actualIds = firstPartitionIds.Concat(secondPartitionIds).ToArray();
            
            Assert.AreEqual(expectedIds, actualIds);
        }
        
        [UnityTest]
        public IEnumerator GetByAppIds_Over10ElementsTest()
        {
            yield return LoginAsAdminTester();
            
            var appIdToFind = new List<string> { RGNCoreBuilder.I.AppIDForRequests };
            var countOffers = 15;

            var createdOffers = new string[countOffers];
            for (int i = 0; i < countOffers; i++)
            {
                var addStoreOfferTask = AddStoreOfferAsync(appIdToFind);
                yield return addStoreOfferTask.AsIEnumeratorReturnNull();
                var addStoreOfferResult = addStoreOfferTask.Result;
                createdOffers[i] = addStoreOfferResult.id;
            }

            var getOffersTask = StoreModule.I.GetByAppIdsAsync(appIdToFind, 20);
            yield return getOffersTask.AsIEnumeratorReturnNull();
            var getOffersResult = getOffersTask.Result;

            for (int i = 0; i < countOffers; i++)
            {
                yield return DeleteStoreOfferAsync(createdOffers[i]);
            }

            Assert.GreaterOrEqual(getOffersResult.Count, createdOffers.Length);
        }
        
        [UnityTest]
        public IEnumerator GetWithVirtualItemsDataByAppIdsAsync_ReturnsArrayOfOffers()
        {
            yield return LoginAsAdminTester();
            
            var appIdsToFind = new List<string> { "GetWithVirtualItemsDataByAppIdsAsync_ReturnsArrayOfOffers" };

            var addStoreOfferTask = AddStoreOfferAsync(appIdsToFind);
            yield return addStoreOfferTask.AsIEnumeratorReturnNull();
            var addStoreOfferResult = addStoreOfferTask.Result;

            var getStoreOffersByAppIdsTask = StoreModule.I
                .GetWithVirtualItemsDataByAppIdsAsync(appIdsToFind, 1);
            yield return getStoreOffersByAppIdsTask.AsIEnumeratorReturnNull();
            var getStoreOffersByAppIdsResult = getStoreOffersByAppIdsTask.Result;

            yield return DeleteStoreOfferAsync(addStoreOfferResult.id);

            Assert.IsNotEmpty(getStoreOffersByAppIdsResult);
            Assert.AreEqual(getStoreOffersByAppIdsResult[0].itemIds[0], getStoreOffersByAppIdsResult[0].GetItems()[0].id);
        }
        
        [UnityTest]
        public IEnumerator GetByIds_ReturnsArrayOfOffers()
        {
            yield return LoginAsAdminTester();

            List<string> idsToFind = new List<string>();

            var addStoreOfferTask = AddStoreOfferAsync();
            yield return addStoreOfferTask.AsIEnumeratorReturnNull();
            var addStoreOfferResult = addStoreOfferTask.Result;

            idsToFind.Add(addStoreOfferResult.id);
            idsToFind.Add(addStoreOfferResult.id);

            var getStoreOffersByIdsTask = StoreModule.I
                .GetByIdsAsync(idsToFind);
            yield return getStoreOffersByIdsTask.AsIEnumeratorReturnNull();
            var getStoreOffersByIdsResult = getStoreOffersByIdsTask.Result;

            yield return DeleteStoreOfferAsync(addStoreOfferResult.id);

            Assert.IsNotEmpty(getStoreOffersByIdsResult);

            var areOfferIdsCorrect = getStoreOffersByIdsResult.All(x => idsToFind.Contains(x.id));

            Assert.True(areOfferIdsCorrect, "Retrieved store offer doesn't much requested any id");

            var noDuplicates = getStoreOffersByIdsResult
                .GroupBy(x => x.id).Any(g => g.Count() <= 1);

            Assert.True(noDuplicates, "Request returns duplicated store offers");
        }

        [UnityTest]
        public IEnumerator GetByIds_Over10ElementsTest()
        {
            yield return LoginAsAdminTester();
            
            var appIdToFind = new List<string> { RGNCoreBuilder.I.AppIDForRequests };
            var countOffers = 15;

            var createdOffers = new List<string>(countOffers);
            for (int i = 0; i < countOffers; i++)
            {
                var addStoreOfferTask = AddStoreOfferAsync(appIdToFind);
                yield return addStoreOfferTask.AsIEnumeratorReturnNull();
                var addStoreOfferResult = addStoreOfferTask.Result;
                createdOffers.Add(addStoreOfferResult.id);
            }

            var getOffersTask = StoreModule.I.GetByIdsAsync(createdOffers);
            yield return getOffersTask.AsIEnumeratorReturnNull();
            var getOffersResult = getOffersTask.Result;

            for (int i = 0; i < countOffers; i++)
            {
                yield return DeleteStoreOfferAsync(createdOffers[i]);
            }

            Assert.AreEqual(getOffersResult.Count, createdOffers.Count);
        }
        
        [UnityTest]
        public IEnumerator GetTags_ReturnsArrayOfOfferTags()
        {
            yield return LoginAsAdminTester();
            
            var expectedTags = new[]
            {
                "testItemTag1", "testItemTag2"
            };

            var addStoreOfferTask = AddStoreOfferAsync();
            yield return addStoreOfferTask.AsIEnumeratorReturnNull();
            var addStoreOfferResult = addStoreOfferTask.Result;

            var getStoreOfferTagsTask = StoreModule.I
                .GetTagsAsync(addStoreOfferResult.id);
            yield return getStoreOfferTagsTask.AsIEnumeratorReturnNull();
            var getStoreOfferTagsResult = getStoreOfferTagsTask.Result;

            yield return DeleteStoreOfferAsync(addStoreOfferResult.id);

            var tagsAreEqual = expectedTags.Length == getStoreOfferTagsResult.Count;
            if (tagsAreEqual)
            {
                for (var i = 0; i < expectedTags.Length; i++)
                {
                    if (expectedTags[i].Equals(getStoreOfferTagsResult[i]))
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

            var newTags = new List<string>
            {
                "tag1",
                "tag2",
                "tag3",
            };
            string appId = RGNCoreBuilder.I.AppIDForRequests;

            var addStoreOfferTask = AddStoreOfferAsync();
            yield return addStoreOfferTask.AsIEnumeratorReturnNull();
            var addStoreOfferResult = addStoreOfferTask.Result;

            var setTagsTask = StoreModule.I.SetTagsAsync(addStoreOfferResult.id, newTags, appId);
            yield return setTagsTask.AsIEnumeratorReturnNull();

            var getStoreOfferTask = GetStoreOfferAsync(addStoreOfferResult.id);
            yield return getStoreOfferTask.AsIEnumeratorReturnNull();
            var getStoreOfferResult = getStoreOfferTask.Result;

            yield return DeleteStoreOfferAsync(addStoreOfferResult.id);

            var tagsAreEqual = newTags.Count == getStoreOfferResult.tags.Count;
            if (tagsAreEqual)
            {
                for (var i = 0; i < newTags.Count; i++)
                {
                    string expectedTag = $"{newTags[i]}_{appId}";
                    string actualTag = getStoreOfferResult.tags[i];
                    
                    if (expectedTag.Equals(actualTag))
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

            var addStoreOfferTask = AddStoreOfferAsync();
            yield return addStoreOfferTask.AsIEnumeratorReturnNull();
            var addStoreOfferResult = addStoreOfferTask.Result;

            var setNameTask = StoreModule.I
                .SetNameAsync(addStoreOfferResult.id, newName);
            yield return setNameTask.AsIEnumeratorReturnNull();

            var getStoreOfferTask = GetStoreOfferAsync(addStoreOfferResult.id);
            yield return getStoreOfferTask.AsIEnumeratorReturnNull();
            var getStoreOfferResult = getStoreOfferTask.Result;

            yield return DeleteStoreOfferAsync(addStoreOfferResult.id);

            Assert.AreEqual(newName, getStoreOfferResult.name, "Name field didn't set properly");
        }

        [UnityTest]
        public IEnumerator SetDescription_ChecksSetName()
        {
            yield return LoginAsAdminTester();

            var newDescription = "New description for test";

            var addStoreOfferTask = AddStoreOfferAsync();
            yield return addStoreOfferTask.AsIEnumeratorReturnNull();
            var addStoreOfferResult = addStoreOfferTask.Result;

            var setDescriptionTask = StoreModule.I
                .SetDescriptionAsync(addStoreOfferResult.id, newDescription);
            yield return setDescriptionTask.AsIEnumeratorReturnNull();

            var getStoreOfferTask = GetStoreOfferAsync(addStoreOfferResult.id);
            yield return getStoreOfferTask.AsIEnumeratorReturnNull();
            var getStoreOfferResult = getStoreOfferTask.Result;

            yield return DeleteStoreOfferAsync(addStoreOfferResult.id);

            Assert.AreEqual(newDescription, getStoreOfferResult.description, "Description field didn't set properly");
        }

        [UnityTest]
        public IEnumerator SetPrices_ChecksSetPrices()
        {
            yield return LoginAsAdminTester();

            var newPrices = new List<PriceInfo>
            {
                new PriceInfo(new List<string>() { RGNCoreBuilder.I.AppIDForRequests },"itemId1", "currency1", 1),
                new PriceInfo(new List<string>() { RGNCoreBuilder.I.AppIDForRequests },"itemId1", "currency2", 1),
            };

            var addStoreOfferTask = AddStoreOfferAsync();
            yield return addStoreOfferTask.AsIEnumeratorReturnNull();
            var addStoreOfferResult = addStoreOfferTask.Result;

            var setPricesTask = StoreModule.I
                .SetPricesAsync(addStoreOfferResult.id, newPrices);
            yield return setPricesTask.AsIEnumeratorReturnNull();

            var getStoreOfferTask = GetStoreOfferAsync(addStoreOfferResult.id);
            yield return getStoreOfferTask.AsIEnumeratorReturnNull();
            var getStoreOfferResult = getStoreOfferTask.Result;

            yield return DeleteStoreOfferAsync(addStoreOfferResult.id);

            var pricesAreEqual = newPrices.Count == getStoreOfferResult.prices.Count;
            if (pricesAreEqual)
            {
                for (var i = 0; i < newPrices.Count; i++)
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

            var randomAppId = Guid.NewGuid().ToString();
            var newTime = TimeInfo.CreateWithStartAndEndTime(0, 1000);

            var addStoreOfferTask = AddStoreOfferAsync(new List<string> { randomAppId });
            yield return addStoreOfferTask.AsIEnumeratorReturnNull();
            var addStoreOfferResult = addStoreOfferTask.Result;

            var setTimeTask = StoreModule.I
                .SetTimeAsync(addStoreOfferResult.id, newTime);
            yield return setTimeTask.AsIEnumeratorReturnNull();

            var getStoreOfferTask = GetStoreOfferAsync(addStoreOfferResult.id);
            yield return getStoreOfferTask.AsIEnumeratorReturnNull();
            var getStoreOfferResult = getStoreOfferTask.Result;

            yield return DeleteStoreOfferAsync(addStoreOfferResult.id);

            Assert.AreEqual(newTime, getStoreOfferResult.time, "Time field didn't set properly");
        }

        [UnityTest]
        public IEnumerator SetImageUrl_ChecksSetImageUrl()
        {
            yield return LoginAsAdminTester();

            var newImageUrl = "New image url for test";

            var addStoreOfferTask = AddStoreOfferAsync();
            yield return addStoreOfferTask.AsIEnumeratorReturnNull();
            var addStoreOfferResult = addStoreOfferTask.Result;

            var setImageUrlTask = StoreModule.I
                .SetImageUrlAsync(addStoreOfferResult.id, newImageUrl);
            yield return setImageUrlTask.AsIEnumeratorReturnNull();

            var getStoreOfferTask = GetStoreOfferAsync(addStoreOfferResult.id);
            yield return getStoreOfferTask.AsIEnumeratorReturnNull();
            var getStoreOfferResult = getStoreOfferTask.Result;

            yield return DeleteStoreOfferAsync(addStoreOfferResult.id);

            Assert.AreEqual(newImageUrl, getStoreOfferResult.imageUrl, "ImageUrl field didn't set properly");
        }

        [UnityTest]
        public IEnumerator DeleteStoreOffer_ConfirmDeleting()
        {
            yield return LoginAsAdminTester();

            var addStoreOfferTask = AddStoreOfferAsync();
            yield return addStoreOfferTask.AsIEnumeratorReturnNull();
            var addStoreOfferResult = addStoreOfferTask.Result;

            var deleteStoreOfferTask = DeleteStoreOfferAsync(addStoreOfferResult.id);
            yield return deleteStoreOfferTask.AsIEnumeratorReturnNull();

            var getStoreOfferTask = GetStoreOfferAsync(addStoreOfferResult.id);
            yield return getStoreOfferTask.AsIEnumeratorReturnNull();
            var getStoreOfferResult = getStoreOfferTask.Result;

            Assert.IsNull(getStoreOfferResult, "Store offer didn't completely deleted");
        }

        [UnityTest]
        public IEnumerator SetProperties_ReturnsPropertiesThatWasSet()
        {
            yield return LoginAsAdminTester();

            var storeOfferId = "NEIoJ3uobAWr4CrFNrW6";
            var propertiesToSet = "{}";

            var task = StoreModule.I.SetPropertiesAsync(storeOfferId, propertiesToSet);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            Assert.AreEqual(propertiesToSet, result);
            Debug.Log(result);
        }

        [UnityTest]
        public IEnumerator GetProperties_ReturnsPropertiesThatWasSetBeforeInDB()
        {
            var storeOfferId = "NEIoJ3uobAWr4CrFNrW6";
            var expectedProperties = "{}";

            var task = StoreModule.I.GetPropertiesAsync(storeOfferId);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            Assert.AreEqual(expectedProperties, result);
            Debug.Log(result);
        }

        #endregion

        #region Common methods

        private Task<StoreOffer> AddStoreOfferAsync(List<string> customAppIds = null)
        {
            var task = StoreModule.I.AddVirtualItemsStoreOfferAsync(
                customAppIds ?? new List<string> { RGNCoreBuilder.I.AppIDForRequests, "anotherAppId" },
                new List<string> { "ed589211-466b-4d87-9c94-e6ba03a10765" },
                "testItemName",
                "testItemDesc",
                new[] { "testItemTag1", "testItemTag2" });
            return task;
        }

        private async Task<StoreOffer> GetStoreOfferAsync(string offerId)
        {
            var task = StoreModule.I.GetByIdsAsync(new List<string> { offerId });
            var result = await task;
            return result.Count > 0 ? result[0] : null;
        }

        private Task DeleteStoreOfferAsync(string offerId)
        {
            return StoreModule.I.DeleteStoreOfferAsync(offerId);
        }

        #endregion
    }
}
