﻿using System;
using System.Globalization;
using System.Xml.Linq;
using Our.Umbraco.FriendlySitemap.Builders;
using Our.Umbraco.FriendlySitemap.Extensions;
using Our.Umbraco.FriendlySitemap.News.Configuration;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Our.Umbraco.FriendlySitemap.News.Builders
{
    internal class NewsSitemapBuilder : SitemapBuilderBase<NewsSitemapConfiguration>
    {
        private readonly XNamespace _xmlns = XNamespace.Get("http://www.google.com/schemas/sitemap-news/0.9");

        private readonly NewsSitemapConfiguration _config;

        public NewsSitemapBuilder(NewsSitemapConfiguration config)
            : base(config)
        {
            _config = config;
        }

        public override XElement BuildUrlSetElement(IPublishedContent node, CultureInfo culture)
        {
            var urlsetElement = base.BuildUrlSetElement(node, culture);

            urlsetElement.AddNamespace("news", _xmlns);

            return urlsetElement;
        }

        public override XElement BuildMetaElement(IPublishedContent node, CultureInfo culture)
        {
            var newsElement = new XElement(_xmlns + "news");

            var title = node.Value<string>(_config.Fields.Title, culture.Name)
                ?? node.Name(culture.Name);

            if (string.IsNullOrWhiteSpace(title) == false)
            {
                newsElement.AddChild("title", title);
            }

            newsElement.AddChild("publication", x => x
                .AddChild("name", _config.PublicationName)
                .AddChild("language", culture.TwoLetterISOLanguageName)
            );

            var publicationDate = node.Value<DateTime?>(_config.Fields.Date, culture.Name)
                ?? node.CreateDate;

            if (publicationDate > DateTime.MinValue)
            {
                newsElement.AddChild("publication_date", publicationDate.ToString("yyyy-MM-dd hh:mm:ss"));
            }

            return newsElement;
        }
    }
}