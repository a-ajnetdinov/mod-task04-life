using System;
using System.Collections.Generic;
using System.Text;

namespace cli_life {
    public class MenuState {
		public Dictionary<string, Action<string>> actions;
		public string text;

		public MenuState(string text, Dictionary<string, Action<string>> actions) {
			this.text = text;
			this.actions = actions;
		}
	}
}
