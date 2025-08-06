using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using My.Framework.MongoDbAccessor.Interface;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace My.Framework.MongoDbAccessor
{
    /// <summary>
    /// MongoDb上下文。
    /// </summary>
    public class MongoDbContext : IMongoService
    {
        /// <summary>
        /// 代表MongoDB数据库，供调用者自由使用
        /// </summary>
        public IMongoDatabase MongoDb { get; set; }


        /// <summary>
        /// 取默认集合操作设置。
        /// </summary>
        /// <returns></returns>
        private MongoCollectionSettings GetDefaultCollectionSettings()
        {
            MongoCollectionSettings collectionSettings = new MongoCollectionSettings()
            {
                AssignIdOnInsert = true,
                GuidRepresentation = GuidRepresentation.CSharpLegacy,
                ReadPreference = ReadPreference.Primary,
                WriteConcern = WriteConcern.WMajority,
                ReadConcern = ReadConcern.Default
            };

            return collectionSettings;
        }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="conncetionString">MongoDB连接串。如： server=127.0.0.1;database=MyNorthwind;uid=admin;pwd=001</param>
        public MongoDbContext(string conncetionString)
        {
            if (string.IsNullOrWhiteSpace(conncetionString))
                return;
            Regex hostRegex = new Regex(@"(?<=server=)[^\s;]+", RegexOptions.IgnoreCase);
            string host = hostRegex.Match(conncetionString).Value;

            Regex databaseRegex = new Regex(@"(?<=database=)[^\s;]+", RegexOptions.IgnoreCase);
            string database = databaseRegex.Match(conncetionString).Value;

            Regex userRegex = new Regex(@"(?<=uid=)[^\s;]+", RegexOptions.IgnoreCase);
            string user = userRegex.Match(conncetionString).Value;

            Regex useDatabaseRegex = new Regex(@"(?<=usedb=)[^\s;]+", RegexOptions.IgnoreCase);
            string useDatabase = useDatabaseRegex.Match(conncetionString).Value;

            Regex pwdRegex = new Regex(@"(?<=pwd=)[^\s;]+", RegexOptions.IgnoreCase);
            string pwd = pwdRegex.Match(conncetionString).Value;

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(database))
            {
                throw new Exception("参数host和/或database为空");
            }

            string FormalConnStr = host + "/" + database;
            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pwd))
            {
                FormalConnStr = "mongodb://" + FormalConnStr;
            }
            else
            {
                FormalConnStr = "mongodb://" + user + ":" + pwd + "@" + FormalConnStr;
            }

            // 首先创建一个连接
            MongoClient mongo = new MongoClient(FormalConnStr);

            MongoDb = mongo.GetDatabase(string.IsNullOrWhiteSpace(useDatabase) ? database : useDatabase);
        }



        /// <summary>
        /// 返回可查询泛型对象。
        /// </summary>
        /// <typeparam name="T">返回类型。</typeparam>
        /// <param name="isForcingPrimary">是否必须查分片主节点。</param>
        /// <returns>可查接口。</returns>
        public IQueryable<T> GetQueryable<T>(bool isForcingPrimary = true) where T : class, new()
        {
            var collectionSettings = GetDefaultCollectionSettings();
            collectionSettings.ReadPreference = isForcingPrimary ? ReadPreference.Primary : ReadPreference.SecondaryPreferred;

            IQueryable<T> theQuery = MongoDb.GetCollection<T>(typeof(T).FullName, collectionSettings).AsQueryable<T>();

            return theQuery;
        }

        /// <summary>
        /// 添加一个实体。
        /// </summary>
        /// <typeparam name="T">要添加的实体类型。</typeparam>
        /// <param name="entity">要添加的实体。</param>
        public void Add<T>(T entity)
        {
            var theCollection = MongoCollection<T>();
            theCollection.InsertOne(entity);
        }

        private IMongoCollection<T> MongoCollection<T>()
        {
            IMongoCollection<T> theCollection = MongoDb.GetCollection<T>(typeof(T).FullName, GetDefaultCollectionSettings());
            return theCollection;
        }

        /// <summary>
        /// 异步添加一个实体。
        /// </summary>
        /// <typeparam name="T">要添加的实体类型。</typeparam>
        /// <param name="entity">要添加的实体。</param>

        public async Task AddAsync<T>(T entity)
        {
            var collection = MongoCollection<T>();
            await collection.InsertOneAsync(entity);
        }

        /// <summary>
        /// 添加多个同类型的实体。
        /// </summary>
        /// <typeparam name="T">要添加的实体类型。</typeparam>
        /// <param name="entities">要添加的实体列表。</param>
        public void AddRange<T>(IEnumerable<T> entities) where T : class, new()
        {
            IMongoCollection<T> theCollection = MongoDb.GetCollection<T>(typeof(T).FullName, GetDefaultCollectionSettings());
            theCollection.InsertMany(entities);
        }

        /// <summary>
        /// 异步添加多个同类型的实体。
        /// </summary>
        /// <typeparam name="T">要添加的实体类型。</typeparam>
        /// <param name="entities">要添加的实体列表。</param>
        public async Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class, new()
        {
            var collection = MongoCollection<T>();
            await collection.InsertManyAsync(entities);
        }

        /// <summary>
        /// 删除与指定参数有相同键值的同类型实体，通常用于删除一条实体。
        /// </summary>
        /// <typeparam name="T">类型参数。</typeparam>
        /// <param name="entity">实体参数。</param>
        /// <param name="keyStatement">键声明。注意：因为一些原因（比如声明的键并不具有唯一性）可能在声明的键下存在多个实体，
        /// 则本操作会将这些实体全部删除。</param>
        /// <returns>删除的实体数。</returns>
        public long Delete<T>(T entity, Expression<Func<T, object>> keyStatement) where T : class, new()
        {
            var deleteWhere = PreDeleteDocument(entity, keyStatement);

            IMongoCollection<T> theCollection = MongoDb.GetCollection<T>(typeof(T).FullName, GetDefaultCollectionSettings());
            var result = theCollection.DeleteOne(deleteWhere);

            return result.DeletedCount;
        }

        /// <summary>
        /// 异步删除与指定参数有相同键值的同类型实体，通常用于删除一条实体。
        /// </summary>
        /// <typeparam name="T">类型参数。</typeparam>
        /// <param name="entity">实体参数。</param>
        /// <param name="keyStatement">键声明。注意：因为一些原因（比如声明的键并不具有唯一性）可能在声明的键下存在多个实体，
        /// 则本操作会将这些实体全部删除。</param>
        /// <returns>删除的实体数。</returns>
        public async Task<long> DeleteAsync<T>(T entity, Expression<Func<T, object>> keyStatement) where T : class, new()
        {
            var deleteWhere = PreDeleteDocument(entity, keyStatement);

            IMongoCollection<T> theCollection = MongoDb.GetCollection<T>(typeof(T).FullName, GetDefaultCollectionSettings());
            var result = await theCollection.DeleteOneAsync(deleteWhere);
            return result.DeletedCount;
        }

        private QueryDocument PreDeleteDocument<T>(T entity, Expression<Func<T, object>> keyStatement) where T : class, new()
        {
            if (entity == null || keyStatement == null)
            {
                throw new Exception("参数entity和/或KeyStatement为空");
            }

            string strRawKSRoute = keyStatement.Body.ToString();
            Regex ksReg = new Regex(@"[^\.]*?\.");
            string routePrefix = ksReg.Match(strRawKSRoute).Value;
            string strKSRoute = strRawKSRoute.Replace(routePrefix, "");
            strKSRoute = strKSRoute.Replace("(", "").Replace(")", "");
            object keyValue = TypeUtil.GetPropertyValueByRoute(entity, strKSRoute);
            if (keyValue == null)
            {
                throw new Exception("指定的键属性不存在或者没有赋值");
            }


            BsonValue bsonKey = BsonValue.Create(keyValue);

            //主键推断对Where子句的影响。

            QueryDocument deleteWhere = null;
            List<string> idList = new List<string>() { "Id", "id", "_id" };
            if (idList.Contains(strKSRoute))
            {
                deleteWhere = new QueryDocument("_id", bsonKey);
            }
            else
            {
                deleteWhere = new QueryDocument(strKSRoute, bsonKey);
            }

            return deleteWhere;
        }

        /// <summary>
        /// 按条件删除，通常用于删除多条实体。
        /// </summary>
        /// <typeparam name="T">要删除的实体类型。</typeparam>
        /// <param name="where">执行删除的条件。</param>
        /// <returns>删除的实体数。</returns>
        public long Delete<T>(Expression<Func<T, bool>> where) where T : class, new()
        {
            if (where == null)
            {
                throw new ArgumentNullException(nameof(where), "where不能为空");
            }
            IMongoCollection<T> theCollection = MongoDb.GetCollection<T>(typeof(T).FullName, GetDefaultCollectionSettings());
            var result = theCollection.DeleteMany(where);
            return result.DeletedCount;
        }

        /// <summary>
        /// 异步按条件删除，通常用于删除多条实体。
        /// </summary>
        /// <typeparam name="T">要删除的实体类型。</typeparam>
        /// <param name="where">执行删除的条件。</param>
        /// <returns>删除的实体数。</returns>
        public async Task<long> DeleteAsync<T>(Expression<Func<T, bool>> where) where T : class, new()
        {
            if (where == null)
            {
                throw new ArgumentNullException(nameof(where), "where不能为空");
            }
            IMongoCollection<T> theCollection = MongoDb.GetCollection<T>(typeof(T).FullName, GetDefaultCollectionSettings());
            var result = await theCollection.DeleteManyAsync(where);
            return result.DeletedCount;
        }

        /// <summary>
        /// 将存在相同键值的实体更新为指定实体。
        /// </summary>
        /// <typeparam name="T">类型参数。</typeparam>
        /// <param name="entity">实体参数。</param>
        /// <param name="keyStatement">键声明。注意：因为一些原因（比如声明的键并不具有唯一性）可能在声明的键下存在多个实体，
        /// 则本操作会将多余实体全部删除。只更新和保留其中一个。</param>
        /// <returns>受影响的实体数。</returns>
        public long Update<T>(T entity, Expression<Func<T, object>> keyStatement) where T : class, new()
        {
            if (null == entity)
                return 0;

            long itemAffected = 0;
            itemAffected = Delete(entity, keyStatement);
            if (itemAffected > 0)
            {
                Add(entity);
            }
            return itemAffected;
        }

        /// <summary>
        /// 异步将存在相同键值的实体更新为指定实体。
        /// </summary>
        /// <typeparam name="T">类型参数。</typeparam>
        /// <param name="entity">实体参数。</param>
        /// <param name="keyStatement">键声明。注意：因为一些原因（比如声明的键并不具有唯一性）可能在声明的键下存在多个实体，
        /// 则本操作会将多余实体全部删除。只更新和保留其中一个。</param>
        /// <returns>受影响的实体数。</returns>
        public async Task<long> UpdateAsync<T>(T entity, Expression<Func<T, object>> keyStatement) where T : class, new()
        {
            if (null == entity)
                return 0;

            long itemAffected = 0;
            itemAffected = await DeleteAsync(entity, keyStatement);
            if (itemAffected > 0)
            {
                Add(entity);
            }
            return itemAffected;
        }

        /// <summary>
        /// 按条件修改实体对象的部分字段。
        /// </summary>
        /// <typeparam name="T">要更新的实体类型。</typeparam>
        /// <param name="update">要更改的字段表达式。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="updateInc">要自增操作的表达式</param>
        /// <returns>受影响的记录数。</returns>
        public long Update<T>(Expression<Func<T>> update, Expression<Func<T, bool>> where, Expression<Func<T>> updateInc = null) where T : class, new()
        {
            if (null == update)
                return 0;

            if (where == null)
            {
                throw new ArgumentNullException(nameof(where), "where不能为空");
            }
            BsonDocument updateSet = TranslateUpdate(update, updateInc);

            IMongoCollection<T> theCollection = MongoDb.GetCollection<T>(typeof(T).FullName, GetDefaultCollectionSettings());
            var result = theCollection.UpdateMany(where, updateSet);
            return result.ModifiedCount;
        }

        /// <summary>
        /// 异步按条件修改实体对象的部分字段。
        /// </summary>
        /// <typeparam name="T">要更新的实体类型。</typeparam>
        /// <param name="update">要更改的字段表达式。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="updateInc">要自增操作的表达式</param>
        /// <returns>受影响的记录数。</returns>
        public async Task<long> UpdateAsync<T>(Expression<Func<T>> update, Expression<Func<T, bool>> where, Expression<Func<T>> updateInc = null) where T : class, new()
        {
            if (null == update)
                return 0;

            if (where == null)
            {
                throw new ArgumentNullException(nameof(where), "where不能为空");
            }
            BsonDocument updateSet = TranslateUpdate(update, updateInc);

            IMongoCollection<T> theCollection = MongoDb.GetCollection<T>(typeof(T).FullName, GetDefaultCollectionSettings());
            var result = await theCollection.UpdateManyAsync(where, updateSet);
            return result.ModifiedCount;
        }

        /// <summary>
        /// 合并插入和更新，当存在相同键值的实体则执行更新，否则执行插入。
        /// </summary>
        /// <typeparam name="T">类型参数。</typeparam>
        /// <param name="entity">实体参数。</param>
        /// <param name="keyStatement">键声明。注意：因为一些原因（比如声明的键并不具有唯一性）可能在声明的键下存在多个实体，
        /// 则本操作会将多余实体全部删除。只更新和保留其中一个。</param>
        /// <returns>受影响的实体数。</returns>
        public void Set<T>(T entity, Expression<Func<T, object>> keyStatement) where T : class, new()
        {
            Delete(entity, keyStatement);
            Add(entity);
        }

        /// <summary>
        /// 翻译Update部分的Lambda表达式。得到要更新的属性或字段。
        /// </summary>
        /// <typeparam name="T">使用条件的类型参数。</typeparam>
        /// <param name="update">Update表达式。</param>
        /// <param name="updateInc">要自增操作的表达式</param>
        /// <returns>翻译结果。</returns>
        private BsonDocument TranslateUpdate<T>(Expression<Func<T>> update, Expression<Func<T>> updateInc) where T : new()
        {
            UpdateBuilder mongoUpdate = new UpdateBuilder();

            if (update != null)
            {
                if (update.NodeType == ExpressionType.Lambda)
                {
                    MemberInitExpression updatebody = update.Body as MemberInitExpression;
                    InitExpresPropAnalysiser analysiser = new InitExpresPropAnalysiser();
                    List<MemberInform> updatedPropList = analysiser.Analysiser(updatebody);

                    foreach (var membVal in updatedPropList)
                    {
                        BsonDocumentWrapper bsonDocumentWrapper = BsonDocumentWrapper.Create(membVal.ValueType, membVal.Value);
                        var bsonValue = (BsonValue)bsonDocumentWrapper;

                        mongoUpdate.Set(membVal.MemberRoute, bsonValue);
                    }
                }
                else
                {
                    throw new ArgumentNullException(nameof(update), "参数Update不是正确的Lambda表达式");
                }
            }

            if (updateInc != null)
            {
                if (updateInc.NodeType == ExpressionType.Lambda)
                {
                    MemberInitExpression updateIncbody = updateInc.Body as MemberInitExpression;
                    InitExpresPropAnalysiser incAnalysiser = new InitExpresPropAnalysiser();
                    List<MemberInform> incPropList = incAnalysiser.Analysiser(updateIncbody);

                    foreach (var inform in incPropList)
                    {
                        mongoUpdate.Inc(inform.MemberRoute, Convert.ToInt64(inform.Value));
                    }
                }
                else
                {
                    throw new ArgumentNullException(nameof(updateInc), "参数UpdateInc不是正确的Lambda表达式");
                }
            }
            return mongoUpdate.ToBsonDocument();
        }


        /// <summary>
        /// 按条件将指定对象做自增操作。
        /// </summary>
        /// <typeparam name="T">要更新的实体类型。</typeparam>
        /// <param name="updateInc">要自增操作的表达式</param>
        /// <param name="where">条件表达式。</param>
        /// <returns>受影响的记录数。</returns>
        public long UpdateInc<T>(Expression<Func<T>> updateInc, Expression<Func<T, bool>> where) where T : class, new()
        {
            if (null == updateInc)
                return 0;

            if (where == null)
            {
                throw new ArgumentNullException("where不能为空");
            }
            BsonDocument updateSet = TranslateUpdate(null, updateInc);
            IMongoCollection<T> theCollection = MongoDb.GetCollection<T>(typeof(T).FullName, GetDefaultCollectionSettings());
            var result = theCollection.UpdateMany(where, updateSet);
            return result.ModifiedCount;
        }

        /// <summary>
        /// 异步按条件将指定对象做自增操作。
        /// </summary>
        /// <typeparam name="T">要更新的实体类型。</typeparam>
        /// <param name="updateInc">要自增操作的表达式</param>
        /// <param name="where">条件表达式。</param>
        /// <returns>受影响的记录数。</returns>
        public async Task<long> UpdateIncAsync<T>(Expression<Func<T>> updateInc, Expression<Func<T, bool>> where) where T : class, new()
        {
            if (null == updateInc)
                return 0;

            if (where == null)
            {
                throw new ArgumentNullException("where不能为空");
            }
            BsonDocument updateSet = TranslateUpdate(null, updateInc);
            IMongoCollection<T> theCollection = MongoDb.GetCollection<T>(typeof(T).FullName, GetDefaultCollectionSettings());
            var result = await theCollection.UpdateManyAsync(where, updateSet);
            return result.ModifiedCount;
        }

        /// <summary>
        /// 找到一个单一的文件，并原子化更新
        /// </summary>
        /// <typeparam name="T">要更新的实体类型。</typeparam>
        /// <param name="update">要更改的字段表达式。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="updateInc">要自增操作的表达式</param>
        /// <param name="returnDocumentAfter">是否返回操作后版本</param>
        /// <returns>returnDocumentAfter 为true返回更新后文档,returnDocumentAfter为false返回更新前文档</returns>
        public T FindOneAndUpdate<T>(Expression<Func<T>> update, Expression<Func<T, bool>> where, Expression<Func<T>> updateInc = null, bool returnDocumentAfter = true) where T : class, new()
        {
            if (null == update && updateInc == null)
                return null;

            if (where == null)
            {
                throw new ArgumentNullException("where不能为空");
            }
            BsonDocument updateSet = TranslateUpdate(update, updateInc);

            var theCollection = MongoCollection<T>();
            var options = new FindOneAndUpdateOptions<T, T> { ReturnDocument = returnDocumentAfter ? ReturnDocument.After : ReturnDocument.Before };
            var result = theCollection.FindOneAndUpdate(where, updateSet, options);
            return result;
        }

        /// <summary>
        /// 异步找到一个单一的文件，并原子化更新
        /// </summary>
        /// <typeparam name="T">要更新的实体类型。</typeparam>
        /// <param name="update">要更改的字段表达式。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="updateInc">要自增操作的表达式</param>
        /// <param name="returnDocumentAfter">是否返回操作后版本</param>
        /// <returns>returnDocumentAfter 为true返回更新后文档,returnDocumentAfter为false返回更新前文档</returns>
        public async Task<T> FindOneAndUpdateAsync<T>(Expression<Func<T>> update, Expression<Func<T, bool>> where, Expression<Func<T>> updateInc = null, bool returnDocumentAfter = true) where T : class, new()
        {
            if (null == update && updateInc == null)
                return null;

            if (where == null)
            {
                throw new ArgumentNullException("where不能为空");
            }
            BsonDocument updateSet = TranslateUpdate(update, updateInc);

            var theCollection = MongoCollection<T>();
            var options = new FindOneAndUpdateOptions<T, T> { ReturnDocument = returnDocumentAfter ? ReturnDocument.After : ReturnDocument.Before };
            var result = await theCollection.FindOneAndUpdateAsync(where, updateSet, options);
            return result;
        }
    }
}
