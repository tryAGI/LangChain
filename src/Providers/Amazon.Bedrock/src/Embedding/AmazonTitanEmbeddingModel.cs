﻿using System.Diagnostics;
using System.Text.Json.Nodes;
using LangChain.Providers.Amazon.Bedrock.Internal;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock;

public abstract class AmazonTitanEmbeddingModel(
    BedrockProvider provider,
    string id)
    : Model<EmbeddingSettings>(id), IEmbeddingModel
{

    /// <summary>
    /// Creates embeddings for the input strings using the Amazon model.
    /// </summary>
    /// <param name="request">The `EmbeddingRequest` containing the input strings.</param>
    /// <param name="settings">Optional `EmbeddingSettings` to override the model's default settings.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>An `EmbeddingResponse` containing the generated embeddings and usage information.</returns>

    public async Task<EmbeddingResponse> CreateEmbeddingsAsync(
        EmbeddingRequest request,
        EmbeddingSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));

        var watch = Stopwatch.StartNew();

        var usedSettings = AmazonEmbeddingSettings.Calculate(
            requestSettings: settings,
            modelSettings: Settings,
            providerSettings: provider.EmbeddingSettings);

        var splitText = request.Strings.Split(chunkSize: (int)usedSettings.MaximumInputLength!);
        var embeddings = new List<float[]>(capacity: splitText.Count);

        var tasks = splitText.Select(text => provider.Api.InvokeModelAsync(Id, new JsonObject { { "inputText", text }, }, cancellationToken))
            .ToList();
        var results = await Task.WhenAll(tasks).ConfigureAwait(false);

        foreach (var response in results)
        {
            var embedding = response?["embedding"]?.AsArray();
            if (embedding == null) continue;

            var f = new float[(int)usedSettings.Dimensions!];
            for (var i = 0; i < embedding.Count; i++)
            {
                f[i] = (float)embedding[(Index)i]?.AsValue()!;
            }

            embeddings.Add(f);
        }

        var usage = Usage.Empty with
        {
            Time = watch.Elapsed,
        };
        AddUsage(usage);
        provider.AddUsage(usage);

        return new EmbeddingResponse
        {
            Values = embeddings.ToArray(),
            Usage = Usage.Empty,
            UsedSettings = usedSettings,
        };
    }
}