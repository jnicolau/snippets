using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using Rhino.Mocks;
using System.IO;

namespace PaymentsWeb.UnitTests.Helpers
{
    static class FakeHttpContext
    {

        public static HttpContextBase Generic()
        {
            var httpContext = MockRepository.GenerateMock<HttpContextBase>();
            var httpRequest = MockRepository.GenerateMock<HttpRequestBase>();
            httpRequest.Expect((r) => r.QueryString).Return(new NameValueCollection());
            httpContext.Expect(c => c.Request).Return(httpRequest).Repeat.Any();
            return httpContext;
        }

        public const string PostDataForThreeDSecureReturnAction =
            "MD=NDEyYTQyNTBmOTkzNmE1YTE5NGI1NDRkYzM2OWMzN2U2ZmZmYjg0OA%3D%3D&PaRes=12345eJytV1u3ojgT%2FSusnkenm5uo9OK4VgIIqEFBwMsbAnIVjgIC%2FvoJ2uf0mZ6envkuvEjKql2Vys5OEKzoGgTSJvDqazAVUFCWbhgQsf%2Fyae2awYWmhyzDTMYTfsJSzJBiWI4aMvSQ58Y8OxpOPk2FNTCD8hHBTK6Kn%2FKB166BdmOL5iDLbUcB%2FFDY8RZcy7jIp%2FQX6gsjkG9DnPTqRW5eTQXXu0BNnw5phh1yAvltKJyDqyZNH8YRroT69vfTLJDf49d1%2F1biibSxP0WWdtclm9HvIYckuUN3g9YTb7iywItA9h6C71bBlKGoEcUyQ4Lmv7LsV4oXyIddeO3hwLmoMTZPUZRAfrQIuGXXIPe6KcWOBPJ9JATta5EH2ANP8v1dIL8X9%2BrmU%2BrDw7I0du2tgrWbClV8%2FktRHM7wsAtl5VZ1Od0L5Lc3wXNvtynuMQS7%2BVyRF0bfcJCme%2FB88GQfLkLgxVMKN67%2FfUSBLCyucRWd%2B1L%2FbBDIvhTysbhTYROHOU52DYj2nOXly6eoql6%2FkmTTNF8a9ktxDUlcMEVSPIkd%2FDIOf%2Fv0jAp8LT8VU0F08yKPPTeL726Flx0FVVT4xHvCn0FaZo9Kk6Ysfsawnz16mH%2FuLRRLcxif%2FDnoh3L%2FTZYfC7%2BW7ucycuk%2BwQ9AU8EMTkG%2FzAFhm9rLp9%2F%2BgfJSHAZl9d9U8VbBR4Q3PMfN6mBaGi11k%2BfMbMJJxzu0yI4q68U2ZEX08hb39BTI97K%2FzeltVd7n93SMmGFrjSl6rTm32CurGKBTt2yK28IYLNl0zJ7PsySvTvEqPqhMovh0Y6jcYTEaLNttwUgTfd%2FmpVHuurr2RrDW01u0JeNokxWHxWQGB96FkYsxuQjWV%2F5gc9GJvBklgtLKORdWPUZGCtrE9IyjmDb0ZCGlZ0vekcxk0M4nRRkrJD22tq8HM%2B2n%2BEPxwiLonrPacRQvuZX7fNvUxyTwKt3Fu0fUX2hmjPWH%2BkL%2FTqzsF80iNsEVTzYo8fhFWbvdGW9XPFi%2BbDo%2FD7rfiY31ogcNsSnqKiK2bta7ii%2FAFsgf4R%2F5xOBaxSfMSiwhSNOkJhFFwHQhaDQIQk1TKpum%2Bz2qyI1k7OeL4qBFN08HhixDAzSnRF4ikCqAtmUYIdFw7FaRwJaAoe5gBEuiuMzvoHVkHcoV4W6%2FbTPvLq8RoJ5BLVJsNkuPZ4dDZtEoYC85hrGQ25lqKzPuuHVqwldRaKsmp8ktdZBkG0HtESy2aLlxjNBmnM4%2FZ8lhh1o5AcYzObJE2zf2u6w6bDnKu4Ng1lANgRKsrpKNFdbmkDV3e6OeyC1K0ka3EK1LpovUfSvewfwJtLdAOnOQGTaz8FGdJjf86rDzQsJmeNpXwtBhZmWfXboD%2FRnkWTDTuYPCZYElbxAEz4ojNDdpGAVbOjvm5h2ZRiM%2FQZeE3FRzR4TWYTdn3K3e9%2Bk9ELRojs8IdpmAGonDFmnKqe%2BhuLkoG%2B3ISoYMgWEDMFR0IImQiI0FDA1xMJBPu8KcdFmiF6%2FbxZnMc%2FnSwTSM5h7f1qtB7aT7y9qqa20xWsycVlXsAbTYwQZUa2bBIUOzCfE15r0R2Xmj5ro%2FzW5qt%2BXz02gZt6AEwYGRYiQeThJt2U0BqNkqaWQPFlmWDDfpZZUfYpdPa9V5TaNlTKzaVp2jcFdr3bk7bSUxilN1QA6XjSYBA8BiqMFUwjzcN014fHDL15oNLC0RXgtgXxRqftl0ocUaIoGy80x9HbRDGiWnuxiF%2B1Ua7v%2BemL%2FiJfGfEvNXvCT%2BNTGHmuHeHXbo3PfktQW%2BTd5S83i%2FJpDr%2FVVzIhNeAhskbUM0Am0U84Wvms0qntycNLMM25zbMS9h2lA2O2eWFKZObs6O%2Bfx2VJrbR74tz3pHHCXAFj3YcN8ezll5sIrbkm69d7CsfQfbyR%2FBWu8nYNCCYXiFoTyDhodJaKpIthuj2WuLZg%2BhYasILBSdiShfBaNlx283jg5NOdt8q9Ty8v3NwK0ifNmpvTNfHsXvTjvqQ686LvFy42dSJBmGeF1X6mntvOqbVbIg5g57Izl5t48bqZ2HtLqRrm08XoS3E5UYEj2p76PzcAINZn0kG2NHBijdeUa20kBckYtxpUoTMGyUp3R%2BVMqfSae4umPKBts36fybNe3lIb1EaazwDYWbZcyABAfIKBvReBBMwTIwh44l7xHcP9kE0do6O6w202%2F%2Bziy0mR8dFeeOjEnfiD5IlZuDFWzN%2BrBLWyUB%2ByfRCouQPzKtg4avpqGl4jPpg%2FD8k%2B68rTnxUCw43EkWNltyhy%2BnNJZRSp8VvY35wfbLTUj8v06HfhMS%2F%2FvpAL03OSX%2BpKdUCZAciphdd26z5h3%2FgtLVzlz5enOx1JbH3HIjTwlHHZphiY3snC2yMJhc6o4qGI9gciMdV%2BlyDc%2Fqan7Uq3HWaWuQ5bdZsTzxcxDiVgMlWd8Bj0D5UDupMWRMI3y9AOunBGCfE5YBaAEskSqJIPV0DI0thJviaJxflwu49oHWbfd2kI%2FVxO50Ekk%2F4Rs2AVjneDWjmB4s741Yd6WVsOMJPbmuzPX9QHKSLnKjYB74QWGC%2FWZy5h1urQ7T5LhF9eKu085KdCxf2dxva3t0FIngDrzL41vlL5uF%2FH7FId%2BvPd8vRI%2BPoseXXH%2BP%2F%2FiF9weP7Ii5&Password=12345&cancelFlag=1";

        public static HttpContextBase ForThreeDSecureReturnAction()
        {
            var httpContext = MockRepository.GenerateMock<HttpContextBase>();
            var httpRequest = MockRepository.GenerateMock<HttpRequestBase>();
            httpContext.Expect(c => c.Request).Return(httpRequest).Repeat.Any();
            var postDataStream = new MemoryStream(
                Encoding.UTF8.GetBytes(PostDataForThreeDSecureReturnAction));
            httpRequest.Expect(r => r.InputStream).Return(postDataStream);
            return httpContext;
        }

    }
}
