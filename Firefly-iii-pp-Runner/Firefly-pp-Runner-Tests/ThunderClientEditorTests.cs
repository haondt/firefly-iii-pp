using Firefly_iii_pp_Runner.Models.ThunderClient.Enums;
using Firefly_iii_pp_Runner.Services;
using Firefly_iii_pp_Runner.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FireflyIIIpp.Thunder.Tests
{
    public class ThunderClientEditorTests
    {
        private readonly ThunderClientEditorService _sut;

        public ThunderClientEditorTests()
        {
            _sut = new ThunderClientEditorService(Options.Create(new ThunderClientEditorSettings
            {
                Path = "."
            }));
        }

        private Firefly_iii_pp_Runner.Models.Postman.Item PrepareItem(List<string> exec)
        {
            return new Firefly_iii_pp_Runner.Models.Postman.Item
            {
                Event = new List<Firefly_iii_pp_Runner.Models.Postman.Event>
                {
                    new Firefly_iii_pp_Runner.Models.Postman.Event
                    {
                        Listen = Firefly_iii_pp_Runner.Models.Postman.Enums.EventTypeEnum.Test,
                        Script = new Firefly_iii_pp_Runner.Models.Postman.Script
                        {
                            Type = Firefly_iii_pp_Runner.Models.Postman.Enums.ScriptTypeEnum.Javascript,
                            Exec = exec
                        }
                    }
                }
            };
        }


        [Fact]
        public void ParsesPostmanExecCorrectlyWithJsonDataFirst()
        {
            var exec = new List<string>
            {
                "var jsonData = pm.response.json();",
                "pm.test('Destination name', function() {",
                "    pm.expect(jsonData.destination_name).to.eql(\"Dollarama\");",
                "});"
            };
            var (_, result) = _sut.ExtractTests(PrepareItem(exec));
            Assert.Contains(result, r =>
                r.Value == "Dollarama"
                && r.Type == TestTypeEnum.JsonQuery
                && r.Action == TestActionEnum.Equal
                && r.Custom == "json.destination_name");
        }

        [Fact]
        public void ParsesPostmanExecCorrectlyWithJsonDataInTest()
        {
            var exec = new List<string>
            {
                "pm.test('Destination name', function() {",
                "    var jsonData = pm.response.json();",
                "    pm.expect(jsonData.destination_name).to.eql(\"Amazon\");",
                "});"
            };
            var (_, result) = _sut.ExtractTests(PrepareItem(exec));
            Assert.Contains(result, r =>
                r.Value == "Amazon"
                && r.Type == TestTypeEnum.JsonQuery
                && r.Action == TestActionEnum.Equal
                && r.Custom == "json.destination_name");
        }

        [Fact]
        public void ParsesPostmanExecCorrectlyWithMultipleChecks()
        {
            var exec = new List<string>
            {
                "var jsonData = pm.response.json();",
                "pm.test('Destination name', function() {",
                "    pm.expect(jsonData.destination_name).to.eql(\"AMA\");",
                "});",
                "",
                "pm.test('Bill name', function() {",
                "    pm.expect(jsonData.bill_name).to.eql(\"Vehicle Insurance and Registration\");",
                "});"
            };
            var (_, result) = _sut.ExtractTests(PrepareItem(exec));
            Assert.Contains(result, r =>
                r.Value == "AMA"
                && r.Type == TestTypeEnum.JsonQuery
                && r.Action == TestActionEnum.Equal
                && r.Custom == "json.destination_name");
            Assert.Contains(result, r =>
                r.Value == "Vehicle Insurance and Registration"
                && r.Type == TestTypeEnum.JsonQuery
                && r.Action == TestActionEnum.Equal
                && r.Custom == "json.bill_name");
        }

        [Fact]
        public void ParsesPostmanExecCorrectlyWithNonStringValues()
        {
            var exec = new List<string>
            {
                "var jsonData = pm.response.json();",
                "pm.test('Description', function() {",
                "    pm.expect(jsonData.description).to.eql(",
                "        \"Point of Sale - Visa Debit VISA DEBIT RETAIL PURCHASE AHHHH BEEES 1234\");",
                "});",
                "pm.test('Type', function() {",
                "    pm.expect(jsonData.type).to.eql(\"withdrawal\");",
                "});",
                "pm.test('Date', function() {",
                "    pm.expect(jsonData.date).to.eql(\"2022-11-22T00:00:00+01:00\");",
                "});",
                "pm.test('Amount', function() {",
                "    pm.expect(jsonData.amount).to.eql(\"2024.81\");",
                "});",
                "pm.test('Source Id', function() {",
                "    pm.expect(jsonData.source_id).to.eql(\"3\");",
                "});",
                "pm.test('Bill Name', function() {",
                "    pm.expect(jsonData.bill_name).to.eql(\"Groceries\");",
                "});",
                "pm.test('Destination Name', function() {",
                "    pm.expect(jsonData.destination_name).to.eql(\"(no name)\");",
                "});",
                "pm.test('Tags', function() {",
                "    pm.expect(jsonData.tags).to.eql([",
                "        \"beans\"",
                "    ]);",
                "});",
                "pm.test('Original Source', function() {",
                "    pm.expect(jsonData.original_source).to.eql(\"ff3-v5.7.17|api-v1.5.6\");",
                "});",
                "pm.test('Import Hash', function() {",
                "    pm.expect(jsonData.import_hash_v2).to.eql(\"071724920338c87e6f6c7942c8f072fd648038b6e677d23c21fa1b58b4ce7c5b\");",
                "});",
                "pm.test('Notes', function() {",
                "    pm.expect(jsonData.notes).to.eql(\"foo\");",
                "});"
        };
            var (_, result) = _sut.ExtractTests(PrepareItem(exec));
            Assert.Contains(result, r =>
                r.Value == "[\"beans\"]"
                && r.Type == TestTypeEnum.JsonQuery
                && r.Action == TestActionEnum.Equal
                && r.Custom == "json.tags");
            Assert.Contains(result, r =>
                r.Value == "foo"
                && r.Type == TestTypeEnum.JsonQuery
                && r.Action == TestActionEnum.Equal
                && r.Custom == "json.notes");
            Assert.Contains(result, r =>
                r.Value == "2022-11-22T00:00:00+01:00"
                && r.Type == TestTypeEnum.JsonQuery
                && r.Action == TestActionEnum.Equal
                && r.Custom == "json.date");
        }


    }
}
