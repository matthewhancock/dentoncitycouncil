using System;

namespace Site {
    public abstract class Page {
        public abstract string Description { get; }
        public abstract string Header { get; }
        public abstract string Key { get; }
        public abstract string Path { get; }
        public abstract string Title { get; }
        public abstract string TitleNav { get; }
        public abstract string Content { get; }
        public string NavLink {
            get {
                return $"<a id=\"link-{this.Key}\" href=\"/{this.Path}\" data-page=\"{this.Key}\" onclick=\"return link(this)\">{this.TitleNav}</a>";
            }
        }
    }
}