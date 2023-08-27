using System;
using System.Collections.Generic;
using Express;

namespace ViewHtml
{
	public class View
	{
		public static uint ID = 0;
		public string id = "";
		
		public string tag = "div";
		public List<NV> attributes = new List<NV>();
		public List<NV> style = new List<NV>();
		public List<View> children = new List<View>();
		public string content = "";
		
		public View()
		{
			id = $"{this.GetType().Name}_{ID++}";
			attributes = new List<NV>
			(
				new NV[]
				{
					new NV("id", id)
				}
			);
		}
		public View Tag(string TAG)
		{
			tag = TAG;
			return this;
		}
		public View Attributes(NV[] ATTRIBUTES)
		{
			attributes.POP(new List<NV>(ATTRIBUTES));
			return this;
		}
		public View Style(NV[] STYLE)
		{
			style.POP(new List<NV>(STYLE));
			return this;
		}
		public View Children(View[] CHILDREN)
		{
			children = new List<View>(CHILDREN);
			content = "";
			return this;
		}
		public View Children(string CHILDREN)
		{
			children = new List<View>();
			content = CHILDREN;
			return this;
		}
		public View ChildrenAdd(View[] CHILDREN)
		{
			children.AddRange(CHILDREN);
			return this;
		}
		public string AttributesToHtml()
		{
			string r = "";
			attributes.ForEach(n => r += n.name + "=" + '"' + n.value + '"');
			return r;
		}
		public string ChildrenToHtml()
		{
			string r = "";
			if(content == "" || string.IsNullOrWhiteSpace(content))
			{
				children.ForEach(n => r += n.ToString());
				return r;
			}
			else
			{
				return content;
			}
		}
		public string StyleToHtml()
		{
			string r = "";
			style.ForEach(n => r += n.name + ":" + n.value + ';');
			if(r == "" || string.IsNullOrWhiteSpace(r))
			{
				return "";
			}
			else
			{
				return "style=" + '"' + r + '"';
			}
		}
		public override string ToString()
		{
			if(tag == "input" || tag == "img")
			{
				return $"<{tag} {AttributesToHtml()} {StyleToHtml()}/>";
			}
			else
			{
				var find = attributes.Find(n => n.name == "style");
				if(find != null)
				{
					return $"<{tag} {AttributesToHtml()}>{ChildrenToHtml()}</{tag}>";
				}
				else
				{
					return $"<{tag} {AttributesToHtml()} {StyleToHtml()}>{ChildrenToHtml()}</{tag}>";
				}
			}
		}
		public static implicit operator string(View view)
		{
			return view.ToString();
		}
	}
	public static class NvExtension
	{
		public static List<NV> POP (this List<NV> THIS, List<NV> OTHER)
		{
			OTHER.ForEach
			(
				(n) =>
				{
					var find = THIS.FindIndex(f => f.name == n.name);
					if(find == -1)
					{
						THIS.Add(n);
					}
					else
					{
						THIS[find] = n;
					}
				}
			);
			return THIS;
		}
	}	
}