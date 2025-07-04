using System.Collections.Generic;

namespace Diplom.Models
{
    public enum ActionType
    {
        software = 1,
        system_command,
        press_key,
        input_text,
        pause
    }
    public class Scenarios_
    {
        public string name;
        public List<ActionScenarios> actions;

        public Scenarios_(string name, List<ActionScenarios> actions)
        {
            this.name = name;
            this.actions = actions;
        }
    }
    public class ActionScenarios
    {
        public string type;
        public string value;
        public float delay;

        public ActionScenarios(string type, string value, float delay)
        {
            this.type = type;
            this.value = value;
            this.delay = delay;
        }


    }
}
