using System;
using CySim.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace CySim.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> Items)
        {
            List<SelectListItem> List = new List<SelectListItem>();
            SelectListItem sli = new SelectListItem
            {
                Text = "----Select-----",
                Value = "0",

            };
            List.Add(sli);
            foreach (var item in Items)
            {
                sli = new SelectListItem
                {
                    Text = item.GetType().GetProperty("TeamName").GetValue(item, null).ToString(),
                    Value = item.GetType().GetProperty("Id").GetValue(item, null).ToString(),
                };
                List.Add(sli);
            }
            return List;

        }
    }
}

