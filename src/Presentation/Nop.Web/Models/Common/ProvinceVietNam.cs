using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Web.Models.Common
{
    public class ProvinceVietNam
    {
        public static List<string> GetList()
        {
            var listData = new List<string>()
            {
                "An Giang",
                "Bà Rịa - Vũng Tàu",
                "Bắc Giang",
                "Bắc Kạn",
                "Bạc Liêu",
                "Bắc Ninh",
                "Bến Tre",
                "Bình Định",
                "Bình Dương",
                "Bình Phước",
                "Bình Thuận",
                "Cà Mau",
                "Cao Bằng",
                "Đắk Lắk",
                "Đắk Nông",
                "Điện Biên",
                "Đồng Nai",
                "Đồng Tháp",
                "Gia Lai",
                "Hà Giang",
                "Hà Nam",
                "Hà Tĩnh",
                "Hải Dương",
                "Hậu Giang",
                "Hòa Bình",
                "Hưng Yên",
                "Khánh Hòa",
                "Kiên Giang",
                "Kon Tum",
                "Lai Châu",
                "Lâm Đồng",
                "Lạng Sơn",
                "Lào Cai",
                "Long An",
                "Nam Định",
                "Nghệ An",
                "Ninh Bình",
                "Ninh Thuận",
                "Phú Thọ",
                "Quảng Bình",
                "Quảng Nam",
                "Quảng Ngãi",
                "Quảng Ninh",
                "Quảng Trị",
                "Sóc Trăng",
                "Sơn La",
                "Tây Ninh",
                "Thái Bình",
                "Thái Nguyên",
                "Thanh Hóa",
                "Thừa Thiên Huế",
                "Tiền Giang",
                "Trà Vinh",
                "Tuyên Quang",
                "Vĩnh Long",
                "Vĩnh Phúc",
                "Yên Bái",
                "Phú Yên",
                "Cần Thơ",
                "Đà Nẵng",
                "Hải Phòng",
                "Hà Nội",
                "Hồ Chí Minh"
            };
            return listData.OrderBy(_ => _).ToList();
        }

        public static List<SelectListItem> GetSelectListItems(string selectedValue = null)
        {
            var items = new List<SelectListItem>();
            foreach (var province in GetList())
            {
                if (selectedValue.IsNullOrEmpty())
                {
                    if (province.Equals("Đà Nẵng", StringComparison.InvariantCultureIgnoreCase))
                    {
                        items.Add(new SelectListItem() { Selected = true, Text = province, Value = province });
                    }
                    else
                    {
                        items.Add(new SelectListItem() { Text = province, Value = province });
                    }
                }
                else
                {
                    if (selectedValue != null && selectedValue.Equals(province, StringComparison.InvariantCultureIgnoreCase))
                    {
                        items.Add(new SelectListItem() { Selected = true, Text = province, Value = province });
                    }
                    else
                    {
                        items.Add(new SelectListItem() { Text = province, Value = province });
                    }
                }
            }
            items.Insert(0, new SelectListItem() { Value = "", Text = "----------------" });
            return items;
        }
    }
}
