﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestSharp.Portable.Test
{
    [TestClass]
    public class RequestParameterTests
    {
        [TestMethod]
        public void TestParameterCaseSensitive()
        {
            var client = new RestClient("http://localhost");
            client.AddDefaultParameter("param1", "value1");

            var request = new RestRequest();
            request
                .AddParameter("param2", "value2")
                .AddParameter("Param1", "value1.1");

            var uri = client.BuildUri(request).ToString();
            Assert.AreEqual("http://localhost/?param1=value1&param2=value2&Param1=value1.1", uri);
        }

        [TestMethod]
        public void TestOnlyDefaultParameter()
        {
            var client = new RestClient("http://localhost");
            client.AddDefaultParameter("param1", "value1");

            var request = new RestRequest();

            var uri = client.BuildUri(request).ToString();
            Assert.AreEqual("http://localhost/?param1=value1", uri);
        }

        [TestMethod]
        public void TestParameterCaseInsensitive()
        {
            var client = new RestClient("http://localhost")
            {
                DefaultParameterNameComparer = StringComparer.OrdinalIgnoreCase,
            };
            client.AddDefaultParameter("param1", "value1");

            var request = new RestRequest();
            request
                .AddParameter("param2", "value2")
                .AddParameter("Param1", "value1.1");

            var uri = client.BuildUri(request).ToString();
            Assert.AreEqual("http://localhost/?Param1=value1.1&param2=value2", uri);
        }

        [TestMethod]
        public void TestParameterOverride()
        {
            var client = new RestClient("http://localhost");
            client.AddDefaultParameter("param1", "value1");

            var request = new RestRequest();
            request
                .AddParameter("param2", "value2")
                .AddParameter("param1", "value1.1");

            var uri = client.BuildUri(request).ToString();
            Assert.AreEqual("http://localhost/?param1=value1.1&param2=value2", uri);
        }

        [TestMethod]
        public void TestParameterDuplication()
        {
            var client = new RestClient("http://localhost");

            var request = new RestRequest();
            request
                .AddParameter("param1", "value1")
                .AddParameter("param2", "value2")
                .AddParameter("param1", "value1.1");

            var uri = client.BuildUri(request).ToString();
            Assert.AreEqual("http://localhost/?param1=value1.1&param2=value2", uri);
        }

        [TestMethod]
        public void TestParameterDuplicationGetAndQueryString()
        {
            var client = new RestClient("http://localhost");

            var request = new RestRequest();
            request
                .AddParameter("param1", "value1", ParameterType.GetOrPost)
                .AddParameter("param2", "value2")
                .AddParameter("param1", "value1.1", ParameterType.QueryString);

            var uri = client.BuildUri(request).ToString();
            Assert.AreEqual("http://localhost/?param1=value1.1&param2=value2", uri);
        }

        [TestMethod]
        public async Task TestParameterDuplicationPostAndQueryString()
        {
            var client = new RestClient("http://localhost");

            var request = new RestRequest(HttpMethod.Post);
            request
                .AddParameter("param1", "value1", ParameterType.GetOrPost)
                .AddParameter("param2", "value2")
                .AddParameter("param1", "value1.1", ParameterType.QueryString);

            var uri = client.BuildUri(request).ToString();
            Assert.AreEqual("http://localhost/?param1=value1.1", uri);

            var body = await client.GetContent(request).ReadAsStringAsync();
            Assert.AreEqual("param1=value1&param2=value2", body);
        }
    }
}
