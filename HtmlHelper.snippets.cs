		public static MvcHtmlString ValidationFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
		{
            return new MvcHtmlString(
                string.Format(@"<span class=""validation-result"" data-valmsg-for=""{0}""></span>", 
                NameFor(html, expression)));
		}

		public static HtmlString Id(this HtmlHelper html, string name)
		{
			return new HtmlString(html.AttributeEncode(html.ViewData.TemplateInfo.GetFullHtmlFieldId(name)));
		}

		public static HtmlString IdFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
		{
			return Id(html, ExpressionHelper.GetExpressionText(expression));
		}

		public static HtmlString IdForModel(this HtmlHelper html)
		{
			return Id(html, String.Empty);
		}

		public static HtmlString Name(this HtmlHelper html, string name)
		{
			return new HtmlString(html.AttributeEncode(html.ViewData.TemplateInfo.GetFullHtmlFieldName(name)));
		}

		public static HtmlString NameFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
		{
			return Name(html, ExpressionHelper.GetExpressionText(expression));
		}

		public static HtmlString NameForModel(this HtmlHelper html)
		{
			return Name(html, String.Empty);
		}
