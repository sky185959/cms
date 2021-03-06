using System.Collections.Generic;
using System.Text;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.CMS.StlTemplates;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "依申请公开查询", Description = "通过 stl:govPublicQuery 标签在模板中实现依申请公开查询功能")]
    public class StlGovPublicQuery
	{
        private StlGovPublicQuery() { }
        public const string ElementName = "stl:govPublicQuery";

        public const string AttributeStyleName = "styleName";

	    public static SortedList<string, string> AttributeList => new SortedList<string, string>
	    {
	        {AttributeStyleName, "样式名称"}
	    };

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var styleName = string.Empty;

            string inputTemplateString;
            string loadingTemplateString;
            string successTemplateString;
            string failureTemplateString;
            StlInnerUtility.GetTemplateLoadingYesNo(pageInfo, contextInfo.InnerXml, out inputTemplateString, out loadingTemplateString, out successTemplateString, out failureTemplateString);

            foreach (var name in contextInfo.Attributes.Keys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, AttributeStyleName))
                {
                    styleName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
            }

            return ParseImpl(pageInfo, contextInfo, styleName, inputTemplateString, successTemplateString, failureTemplateString);
        }

        public static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string styleName, string inputTemplateString, string successTemplateString, string failureTemplateString)
        {
            pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.Jquery);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BShowLoading);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BjTemplates);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BValidate);

            var styleInfo = TagStyleManager.GetTagStyleInfo(pageInfo.PublishmentSystemId, ElementName, styleName) ??
                            new TagStyleInfo();

            var queryTemplate = new GovPublicQueryTemplate(pageInfo.PublishmentSystemInfo, styleInfo);
            var contentBuilder = new StringBuilder(queryTemplate.GetTemplate(styleInfo.IsTemplate, inputTemplateString, successTemplateString, failureTemplateString));

            StlParserManager.ParseTemplateContent(contentBuilder, pageInfo, contextInfo);
            var parsedContent = contentBuilder.ToString();

            return parsedContent;
        }
	}
}
