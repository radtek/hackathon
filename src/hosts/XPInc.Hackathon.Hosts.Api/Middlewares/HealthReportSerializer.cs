using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.Json;

namespace XPInc.Hackathon.Hosts.Api.Middlewares
{
    public static class HealthReportSerializer
    {
        private static readonly JsonWriterOptions _jsonOptions = new JsonWriterOptions
        {
            SkipValidation = true
        };

        public static ReadOnlySpan<byte> AsSpan(HealthReport report) => AsSpan(report, false);

        public static ReadOnlySpan<byte> AsSpan(HealthReport report, bool verbose)
        {
            if (report == default)
            {
                throw new ArgumentNullException(nameof(report));
            }

            return HealthReportSerializer
                    .ConvertInternal(report, verbose)
                    .AsSpan();
        }

        public static ReadOnlyMemory<byte> AsMemory(HealthReport report) => AsMemory(report, false);

        public static ReadOnlyMemory<byte> AsMemory(HealthReport report, bool verbose)
        {
            if (report == default)
            {
                throw new ArgumentNullException(nameof(report));
            }

            return HealthReportSerializer
                    .ConvertInternal(report, verbose)
                    .AsMemory();
        }

        public static string AsJsonString(HealthReport report) => AsJsonString(report, false);

        public static string AsJsonString(HealthReport report, bool verbose)
        {
            if (report == default)
            {
                throw new ArgumentNullException(nameof(report));
            }

            return Encoding.UTF8.GetString(HealthReportSerializer.ConvertInternal(report, verbose));
        }

        private static byte[] ConvertInternal(HealthReport report, bool verbose)
        {
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream, _jsonOptions);

            writer.WriteStartObject();
            writer.WriteString("status", report.Status.ToString());

            if (verbose)
            {
                writer.WriteString("duration", report.TotalDuration.ToString());
            }

            if (report.Entries.Count > 0)
            {
                writer.WriteStartObject("results");

                var entries = report.Entries.ToArray();

                for (int i = 0; i < entries.Length; i++)
                {
                    writer.WriteStartObject(entries[i].Key);
                    writer.WriteString("status", entries[i].Value.Status.ToString());
                    writer.WriteString("description", entries[i].Value.Description);

                    if (verbose)
                    {
                        SerializeVerboseSection(writer, entries[i]);
                    }

                    if (entries[i].Value.Data.Count > 0)
                    {
                        SerializeDataSection(writer, entries[i]);
                    }

                    writer.WriteEndObject();
                }

                writer.WriteEndObject();
            }

            writer.WriteEndObject();
            writer.Flush();

            return stream.ToArray();
        }

        private static void SerializeVerboseSection(Utf8JsonWriter writer, in KeyValuePair<string, HealthReportEntry> entry)
        {
            writer.WriteString("duration", entry.Value.Duration.ToString());

            var tags = entry.Value.Tags.ToArray();

            if (tags.Length > 0)
            {
                writer.WriteStartArray("tags");

                for (int i = 0; i < tags.Length; i++)
                {
                    writer.WriteStringValue(tags[i]);
                }

                writer.WriteEndArray();
            }
        }

        private static void SerializeDataSection(Utf8JsonWriter writer, in KeyValuePair<string, HealthReportEntry> entry)
        {
            writer.WriteStartObject("data");

            var data = entry.Value.Data.ToArray();

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].Value != default)
                {
                    writer.WritePropertyName(data[i].Key);

                    JsonSerializer.Serialize(writer, data[i].Value, data[i].Value.GetType());
                }
            }

            writer.WriteEndObject();
        }
    }
}
