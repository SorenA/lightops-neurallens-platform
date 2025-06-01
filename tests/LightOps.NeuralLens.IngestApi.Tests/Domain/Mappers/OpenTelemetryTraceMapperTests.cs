using System.Text.Json;
using LightOps.NeuralLens.IngestApi.Domain.Mappers;
using OpenTelemetry.Proto.Collector.Trace.V1;

namespace LightOps.NeuralLens.IngestApi.Tests.Domain.Mappers
{
    public class OpenTelemetryTraceMapperTests
    {
        [Fact]
        public void Map()
        {
            // Arrange
            var request = GetRequest();
            var sut = new OpenTelemetryTraceMapper();

            // Act
            var result = sut.Map(request.ResourceSpans[0]);

            // Assert
            Assert.NotNull(result);
            /*Assert.NotEmpty(result.Spans);
            Assert.NotEmpty(result.Events);
            Assert.Equal(request.ResourceSpans[0].ScopeSpans.Count, result.Spans.Count);*/
        }

        private ExportTraceServiceRequest GetRequest()
        {
            // Read from json file ExportTraceServiceRequest-1.json
            var json = File.ReadAllText("ExportTraceServiceRequest-1.json");
            return JsonSerializer.Deserialize<ExportTraceServiceRequest>(json) ?? throw new InvalidDataException();
        }
    }
}
