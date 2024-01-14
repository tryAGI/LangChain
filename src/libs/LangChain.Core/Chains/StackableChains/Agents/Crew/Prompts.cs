namespace LangChain.Chains.StackableChains.Agents.Crew;

public static class Prompts
{
    public static string Task = @"
Begin! This is VERY important to you, your job depends on it!

Current Task: {task}";


    public static string Memory = @"This is the summary of your work so far:
{memory}";



    public static string Role = @"You are {role}.
{backstory}

Your personal goal is: {goal}";


    public static string Tools = @"
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



    public static string Scratchpad = "Thought:{actions_history}";


//    public static string Memory = @"Це підсумок твоєї роботи на даний момент:
//    {memory}";

//    public static string Task = @"
//Починай!

//Поточна задача: {task}";

//    public static string Role = @"Ти {role}.
//{backstory}

//Твоя персональна ціль: {goal}";

//    public static string Tools = @"
//Ти маєш доступ до наведених нижче інструментів:

//{tools}

//Щоб використовувати інструмент, будь ласка, використовуй наступний формат:

//Thought: Чи мені потрібно використати інструмент? Так
//Action: дія, яку потрібно зробити. Має бути одна з [{tool_names}], просто вкажи назву.
//Action Input: параметр для дії
//Observation: результат дії

//Якщо у тебе є відповідь на твою задачу або якщо тобі не потрібно використовувати інструмент, ти ПОВИНЕН використовувати формат:

//Thought: Чи мені потрібно використати інструмент? Ні
//Final Answer: [твоя відповідь тут]
//";
    public static string TaskExecutionWithMemory => string.Join("\n", Role, Tools, Memory, Task, Scratchpad);
    public static string TaskExecutionWithoutMemory => string.Join("\n", Role, Tools, Task, Scratchpad);
}