using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using NUnit.Framework;

namespace PaymentsWeb.UnitTests.Helpers
{
    public static class AssertMvc
    {
        public static AssertView<ViewResultBase> ViewOf<TView>(TView view) where TView : ActionResult
        {
            Assert.IsInstanceOf<TView>(view, string.Format("Expected an instance that inherits ViewResultBase."));
            Assert.IsInstanceOf<TView>(view, string.Format("Expected an instance of type {0}.", typeof(TView).Name));
            return new AssertView<ViewResultBase>((ViewResultBase)(object)view);
        }

        public class AssertView<TView> where TView : ViewResultBase
        {
            private readonly TView _view;

            public AssertView(TView view)
            {
                _view = view;
            }

            public AssertView<TView> AssertName(string expectedName)
            {
                NUnit.Framework.Assert.AreEqual(expectedName, _view.ViewName, "Expected a view named " + expectedName + " but " + _view.ViewName + " was found. ");
                return this;
            }

            public AssertView<TView> AssertIsDefaultView(string defaultViewName = "")
            {
                NUnit.Framework.Assert.IsTrue(_view.ViewName == string.Empty || _view.ViewName == defaultViewName, "Expected a default view.");
                return this;
            }

            public AssertView<TView> Assert(Action<TView> assert)
            {
                assert(_view);
                return this;
            }

            public AssertModel<TModel, TView> ModelOf<TModel>()
            {
                NUnit.Framework.Assert.IsInstanceOf<TModel>(_view.Model, "Model should be an instance of " + typeof(TModel).Name);
                return new AssertModel<TModel, TView>((TModel)_view.Model, _view);
            }

        }

        public class AssertModel<TModel, TView>
        {
            private readonly TModel _model;
            private readonly TView _view;

            public AssertModel(TModel model, TView view)
            {
                _model = model;
                _view = view;
            }

            public AssertModel<TModel, TView> AssertNotNull()
            {
                NUnit.Framework.Assert.NotNull(_model, "The model can't be null.");
                return this;
            }

            public AssertModel<TModel, TView> AssertEqualsTo(TModel expectedModel)
            {
                NUnit.Framework.Assert.AreEqual(expectedModel, _model, "The model is not equal to the expected model.");
                return this;
            }

            public AssertModel<TModel, TView> Assert(Action<TModel> assert)
            {
                assert(_model);
                return this;
            }

            public AssertModel<TModel, TView> Assert(Action<TModel, TView> assert)
            {
                assert(_model, (TView)_view);
                return this;
            }

        }

    }


}
