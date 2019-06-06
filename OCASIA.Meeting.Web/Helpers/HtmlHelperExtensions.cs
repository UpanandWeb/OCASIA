using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Mvc.Ajax;
public static class HtmlHelperExtensions
{
    /// <summary>
    /// Usage:: @Ajax.RawActionLink("<h1>Test</h1>", "Index", "Home", null, null, null) Or @Html.RawActionLink("<h2>Test</h2>", "Index", "Home", null, null)
    /// </summary>
    /// <param name="ajaxHelper"></param>
    /// <param name="rawHtml"></param>
    /// <param name="action"></param>
    /// <param name="controller"></param>
    /// <param name="routeValues"></param>
    /// <param name="ajaxOptions"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns>Ajax.ActionLink</returns>
    public static MvcHtmlString RawActionLink(this AjaxHelper ajaxHelper, string rawHtml, string action, string controller, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
    {
        //string anchor = ajaxHelper.ActionLink("##holder##", action, controller, routeValues, ajaxOptions, htmlAttributes).ToString();
        //return MvcHtmlString.Create(anchor.Replace("##holder##", rawHtml));
        string holder = Guid.NewGuid().ToString();
        string anchor = ajaxHelper.ActionLink(holder, action, controller, routeValues, ajaxOptions, htmlAttributes).ToString();
        return MvcHtmlString.Create(anchor.Replace(holder, rawHtml));
    }
    /// <summary>
    /// Usage:: @Ajax.RawActionLink("<h1>Test</h1>", "Index", "Home", null, null, null) Or @Html.RawActionLink("<h2>Test</h2>", "Index", "Home", null, null)
    /// </summary>
    /// <param name="ajaxHelper"></param>
    /// <param name="rawHtml"></param>
    /// <param name="action"></param>
    /// <param name="controller"></param>
    /// <param name="routeValues"></param>
    /// <param name="ajaxOptions"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns>Ajax.ActionLink</returns>
    public static MvcHtmlString RawActionLink(this HtmlHelper htmlHelper, string rawHtml, string action, string controller, object routeValues, object htmlAttributes)
    {
        //string anchor = htmlHelper.ActionLink("##holder##", action, controller, routeValues, htmlAttributes).ToString();
        //return MvcHtmlString.Create(anchor.Replace("##holder##", rawHtml));
        string holder = Guid.NewGuid().ToString();
        string anchor = htmlHelper.ActionLink(holder, action, controller, routeValues, htmlAttributes).ToString();
        return MvcHtmlString.Create(anchor.Replace(holder, rawHtml));
    }


    //public static MvcHtmlString ToParagraphs(this HtmlHelper html, string value)
    //{
    //    value = html.Encode(value).Replace("\r", String.Empty);
    //    var arr = value.Split('\n').Where(a => a.Trim() != string.Empty);
    //    var htmlStr = "<p>" + String.Join("</p><p>", arr) + "</p>";
    //    return MvcHtmlString.Create(htmlStr);
    //}

}