﻿using LangChain.Abstractions.Schema;
using LangChain.Chains.HelperChains;

namespace LangChain.Chains.StackableChains.Extensions;

public static class HookExtension
{
    public static T Hook<T>(this T chain, Action<IChainValues> hook) where T : BaseStackableChain
    {
        chain = chain ?? throw new ArgumentNullException(nameof(chain));

        chain.SetHook(hook);
        return chain;
    }
}