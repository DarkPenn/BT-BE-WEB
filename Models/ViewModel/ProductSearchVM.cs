using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList.Mvc;

namespace _24DH110165_MyStore.Models.ViewModel
{
    public class ProductSearchVM
    {
        //Search theo tên,mô tả, loại sản phẩm
        public string SearchTerm { get; set; }

        //Search theo giá
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        //Thứ tự Search
        public string SortOrder { get; set; }

        //Thuộc tính hỗ trợ phân trang
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 10; //số sản phẩm mỗi trang

        //Danh sách sản phẩm đã phân trang
        public PagedList.IPagedList<Product> Products { get; set; }

        //Danh sách thoải điều kiện tìm kiếm
        //public List<Product> Products { get; set; }
    }
}