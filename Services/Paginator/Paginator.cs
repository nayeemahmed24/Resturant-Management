using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Model.View_Model;
using MongoDB.Driver;
using Repository;

namespace Services.Paginator
{
    public class Paginator:IPaginator
    {
        private readonly IMongoRepository _repository;
        public Paginator(IMongoRepository repository)
        {
            _repository = repository;
        }

        public PaginatorModel<Type> GetPaginatedData<Type>(string sortBy, int pageNumber, int pageSize, Expression<Func<Type, bool>> datafilters)
        {
            //IEnumerable<Type> data = documents.Find(docs => true).Skip((pageNumber - 1) * pageSize,).Sort(Builders<Type>.Sort.Descending("_id")).Limit(pageSize).ToEnumerable();
            //int totalDocs = (int)documents.CountDocuments(docs => true);

            IEnumerable<Type> data = _repository.GetItemsPaginated<Type>(pageNumber, pageSize, sortBy,datafilters);
            int totalData = (int)_repository.GetDocsCount<Type>(datafilters);

            PaginatorModel<Type> paginatorModel = new PaginatorModel<Type>();
            paginatorModel.data = data;
            paginatorModel.totalData = totalData;
            //Paginator paginationService = new Paginator(data, totalData, paginatorModel);

            return paginatorModel;
        }
    }
}
