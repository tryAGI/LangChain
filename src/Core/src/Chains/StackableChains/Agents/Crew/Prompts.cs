namespace LangChain.Chains.StackableChains.Agents.Crew;

/// <summary>
/// 
/// </summary>
public static class Prompts
{
    /// <summary>
    /// 
    /// </summary>
    public const string Task = @"
Begin! This is VERY important to you, your job depends on it!

Current Task: {task}";

    /// <summary>
    /// 
    /// </summary>
    public const string Memory = @"This is the summary of your work so far:
{memory}";

    /// <summary>
    /// 
    /// </summary>
    public const string Role = @"You are {role}.
{backstory}

Your personal goal is: {goal}";

    /// <summary>
    /// 
    /// </summary>
    public const string Tools = @"
You have access to the following tools:

{tools}

To use a tool, please use the exact following format:

Thought: Do I need to use a tool? Yes
Action: the action to take, should be one of [{tool_names}], just the name.
Action Input: the input to the action
Observation: the result of the action

When you have a response for your task, or if you do not need to use a tool, you MUST use the format:

Thought: Do I need to use a tool? No
Final Answer: [your response here. should be a simple sentence without explanations.]";

    /// <summary>
    /// 
    /// </summary>
    public const string Scratchpad = "Thought:{actions_history}";

    /// <summary>
    /// 
    /// </summary>
    public static string TaskExecutionWithMemory => string.Join("\n", Role, Tools, Memory, Task, Scratchpad);

    /// <summary>
    /// 
    /// </summary>
    public static string TaskExecutionWithoutMemory => string.Join("\n", Role, Tools, Task, Scratchpad);
}