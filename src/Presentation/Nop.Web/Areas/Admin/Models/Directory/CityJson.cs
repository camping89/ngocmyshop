using Newtonsoft.Json;
using System.Collections.Generic;

namespace Nop.Web.Areas.Admin.Models.Directory
{
    public class LtsItem
    {
        public LtsItem()
        {
            DistrictJsons = new List<DistrictJson>();
        }
        public int Type { get; set; }
        public string SolrID { get; set; }
        public int ID { get; set; }
        public string Title { get; set; }
        public int STT { get; set; }
        public object Created { get; set; }
        public object Updated { get; set; }
        public int TotalDoanhNghiep { get; set; }
        [JsonIgnore]
        public List<DistrictJson> DistrictJsons { get; set; }
    }

    public class CityJson
    {
        public List<LtsItem> LtsItem { get; set; }
        public int TotalDoanhNghiep { get; set; }
    }

    public class DistrictJson
    {
        public int Type { get; set; }
        public string SolrID { get; set; }
        public int ID { get; set; }
        public string Title { get; set; }
        public int STT { get; set; }
        public int TinhThanhID { get; set; }
        public string TinhThanhTitle { get; set; }
        public string TinhThanhTitleAscii { get; set; }
        public object Created { get; set; }
        public object Updated { get; set; }
    }
}
