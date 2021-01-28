using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using CodeEffects.Rule.Common;
using Microsoft.AspNetCore.Hosting;

namespace CodeEffects.Rule.Angular.Demo.Services
{
	public class StorageService
	{
		private IWebHostEnvironment environment;

		public StorageService(IWebHostEnvironment environment)
		{
			this.environment = environment;
		}

		/// <summary>
		/// Returns a collection of evaluation type rules.
		/// </summary>
		/// <returns>List of menu items</returns>
		public List<MenuItem> GetEvaluationRules()
		{
			return LoadRules(true);
		}

		/// <summary>
		/// Returns a collection of all rules that associated with the current IP address
		/// </summary>
		/// <returns>List of menu items</returns>
		public List<MenuItem> GetAllRules()
		{
			// Get both execution and evaluation type rules, merge them, sort them and return the result
			List<MenuItem> rules = LoadRules(true);
			rules.AddRange(LoadRules(false));
			rules.Sort((item1, item2) => item1.DisplayName.CompareTo(item2.DisplayName));
			
			return rules;
		}

		public List<string> GetExecutionRuleIds()
		{
			List<string> ruleIds = new List<string>();
			string path = string.Format(@"{0}\rules\execution\", this.environment.WebRootPath);

			if (Directory.Exists(path))
			{
				foreach (string file in Directory.GetFiles(path))
				{
					var parts = file.Split(@"\");
					var id = parts[parts.Length - 1].Replace(".config", string.Empty);
					ruleIds.Add(id);
				}
			}

			return ruleIds;
		}

		public string GetRuleSetXml()
		{
			XmlDocument ruleSetXml = new XmlDocument();
			ruleSetXml.LoadXml(@"<?xml version='1.0' encoding='utf-8'?><codeeffects xmlns='https://codeeffects.com/schemas/rule/41' xmlns:ui='https://codeeffects.com/schemas/ui/4'></codeeffects>");

			//string path = string.Format(@"{0}\rules\evaluation\", this.environment.WebRootPath);

			//if (Directory.Exists(path))
			//{
			//	foreach (string file in Directory.GetFiles(path))
			//	{
			//		XmlDocument ruleXml = new XmlDocument();
			//		ruleXml.Load(file);
			//		XmlNamespaceManager m = new XmlNamespaceManager(ruleXml.NameTable);
			//		m.AddNamespace("x", ruleXml.DocumentElement.NamespaceURI);
			//		XmlNode ruleNode = ruleXml.SelectSingleNode("/x:codeeffects/x:rule", m);
			//		XmlNode node = ruleSetXml.ImportNode(ruleNode, true);
			//		ruleSetXml.DocumentElement.AppendChild(node);
			//	}
			//}

			string path = string.Format(@"{0}\rules\execution\", this.environment.WebRootPath);

			if (Directory.Exists(path))
			{
				foreach (string file in Directory.GetFiles(path))
				{
					XmlDocument ruleXml = new XmlDocument();
					ruleXml.Load(file);
					XmlNamespaceManager m = new XmlNamespaceManager(ruleXml.NameTable);
					m.AddNamespace("x", ruleXml.DocumentElement.NamespaceURI);
					XmlNode ruleNode = ruleXml.SelectSingleNode("/x:codeeffects/x:rule", m);
					XmlNode node = ruleSetXml.ImportNode(ruleNode, true);
					ruleSetXml.DocumentElement.AppendChild(node);
				}
			}

			return ruleSetXml.OuterXml;
		}

		/// <summary>
		/// Saves the rule as a separate XML file with rule ID as file name
		/// </summary>
		/// <param name="ruleID">ID of the rule (automatically assigned by Code Effects)</param>
		/// <param name="ruleXml">String representation of the rule XML document</param>
		/// <param name="isEval">Indicates if the rule is of evaluation type</param>
		public void SaveRule(string ruleID, string ruleXml, bool isEval)
		{
			// Get the path of the rule XML file
			string file = GetFilePath(ruleID, isEval);

			// If a file with this ruleID already exists, delete it
			if (File.Exists(file))
				File.Delete(file);

			// Make sure that all necessary directories exist
			Directory.CreateDirectory(Path.GetDirectoryName(file));

			// Save this rule as XML file
			File.WriteAllText(file, ruleXml);
		}

		/// <summary>
		///  Loads Rule XML by ID of the rule
		/// </summary>
		/// <param name="ruleId">ID of the rule (automatically assigned by Code Effects)</param>
		/// <returns>String representation of the rule XML document</returns>
		public string LoadRuleXml(string ruleId)
		{
			// First, check if a file with this rule ID exists in evaluation directory
			string file = GetFilePath(ruleId, true);
			
			if (File.Exists(file)) 
				return File.ReadAllText(file);
			else
			{
				// Now check if it exists in the execution directory
				file = GetFilePath(ruleId, false);
				if (File.Exists(file)) 
					return File.ReadAllText(file);
			}

			// No such rule found, return null
			return null;
		}

		/// <summary>
		/// Deletes business rule stored in the storage file
		/// </summary>
		/// <param name="ruleId">ID of the rule (automatically assigned by Code Effects)</param>
		public void DeleteRule(string ruleId)
		{
			// This is a quick way to check if this rule is referenced in other rules
			List<MenuItem> files = GetAllRules();

			foreach (MenuItem f in files)
			{
				if (f.ID == ruleId) 
					continue;
				
				string xml = LoadRuleXml(f.ID);
				
				if (xml.IndexOf(ruleId) > -1)
					throw new Exception("The rule that you are trying to delete is referenced in other rules.");
			}

			// First, check if a file with this rule ID exists in evaluation directory
			string file = GetFilePath(ruleId, true);
			if (File.Exists(file))
			{
				File.Delete(file);
				return;
			}
			else
			{
				// Now check if it exists in the execution directory
				file = GetFilePath(ruleId, false);
				if (File.Exists(file))
				{
					File.Delete(file);
					return;
				}
			}
			// No such rule found. This is unexpected; throw an exception
			throw new Exception("Could not find the rule with ID " + ruleId);
		}

		private string GetFilePath(string ruleID, bool isEval)
		{
			return string.Format(@"{0}\rules\{1}\{2}.config", this.environment.WebRootPath, isEval ? "evaluation" : "execution", ruleID);
		}

		private List<MenuItem> LoadRules(bool evaluationType)
		{
			string path = string.Format(@"{0}\rules\{1}\", this.environment.WebRootPath, evaluationType ? "evaluation" : "execution");

			List<MenuItem> list = new List<MenuItem>();

			if (Directory.Exists(path))
			{
				foreach (string file in Directory.GetFiles(path))
				{
					XmlDocument xml = new XmlDocument();
					xml.Load(file);
					XmlNamespaceManager m = new XmlNamespaceManager(xml.NameTable);
					m.AddNamespace("x", xml.DocumentElement.NamespaceURI);
					XmlNode rule = xml.SelectSingleNode("/x:codeeffects/x:rule", m);
					list.Add(new MenuItem(
						rule.Attributes["id"].Value,
						rule.SelectSingleNode("x:name", m).InnerText,
						rule.SelectSingleNode("x:description", m) == null ? null : rule.SelectSingleNode("x:description", m).InnerText));
				}
			}

			return list;
		}
	}
}
