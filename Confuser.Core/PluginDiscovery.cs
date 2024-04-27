using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Confuser.Core.Properties;
using dnlib.DotNet;

namespace Confuser.Core {
	/// <summary>
	///     Discovers available protection plugins.
	/// </summary>
	public class PluginDiscovery {
		/// <summary>
		///     The default plugin discovery service.
		/// </summary>
		public static readonly PluginDiscovery Instance = new PluginDiscovery();

		/// <summary>
		/// default plugins dir
		/// </summary>
		private static string basePlugInsDir;
		/// <summary>
		///     Initializes a new instance of the <see cref="PluginDiscovery" /> class.
		/// </summary>
		protected PluginDiscovery() {
			basePlugInsDir = Path.Combine(AppContext.BaseDirectory, "Plugins");
			if (!Directory.Exists(basePlugInsDir)) {
				Directory.CreateDirectory(basePlugInsDir);
			}
		}

		/// <summary>
		/// get default plugins dir
		/// </summary>
		/// <returns></returns>
		public string GetBasePlugInsDir() {
			return basePlugInsDir;
		}

		/// <summary>
		///     Retrieves the available protection plugins.
		/// </summary>
		/// <param name="context">The working context.</param>
		/// <param name="protections">A list of resolved protections.</param>
		/// <param name="packers">A list of resolved packers.</param>
		/// <param name="components">A list of resolved components.</param>
		public void GetPlugins(ConfuserContext context, out IList<Protection> protections, out IList<Packer> packers, out IList<ConfuserComponent> components) {
			protections = new List<Protection>();
			packers = new List<Packer>();
			components = new List<ConfuserComponent>();
			GetPluginsInternal(context, protections, packers, components);
		}

		/// <summary>
		///     Determines whether the specified type has an accessible default constructor.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns><c>true</c> if the specified type has an accessible default constructor; otherwise, <c>false</c>.</returns>
		public static bool HasAccessibleDefConstructor(Type type) {
			ConstructorInfo ctor = type.GetConstructor(Type.EmptyTypes);
			if (ctor == null) return false;
			return ctor.IsPublic;
		}

		/// <summary>
		///     Adds plugins in the assembly to the protection list.
		/// </summary>
		/// <param name="context">The working context.</param>
		/// <param name="protections">The working list of protections.</param>
		/// <param name="packers">The working list of packers.</param>
		/// <param name="components">The working list of components.</param>
		/// <param name="asm">The assembly.</param>
		protected static void AddPlugins(
			ConfuserContext context, IList<Protection> protections, IList<Packer> packers,
			IList<ConfuserComponent> components, Assembly asm) {
			foreach(var module in asm.GetLoadedModules())
				foreach (var i in module.GetTypes()) {
					if (i.IsAbstract || !HasAccessibleDefConstructor(i))
						continue;

					if (typeof(Protection).IsAssignableFrom(i)) {
						try {
							protections.Add((Protection)Activator.CreateInstance(i));
						}
						catch (Exception ex) {
							context.Logger.ErrorException(string.Format(Resources.PluginDiscovery_AddPlugins_Failed_to_instantiate_protection, i.Name), ex);
						}
					}
					else if (typeof(Packer).IsAssignableFrom(i)) {
						try {
							packers.Add((Packer)Activator.CreateInstance(i));
						}
						catch (Exception ex) {
							context.Logger.ErrorException(string.Format(Resources.PluginDiscovery_AddPlugins_Failed_to_instantiate_packer, i.Name), ex);
						}
					}
					else if (typeof(ConfuserComponent).IsAssignableFrom(i)) {
						try {
							components.Add((ConfuserComponent)Activator.CreateInstance(i));
						}
						catch (Exception ex) {
							context.Logger.ErrorException(string.Format(Resources.PluginDiscovery_AddPlugins_Failed_to_instantiate_component, i.Name), ex);
						}
					}
				}
			context.CheckCancellation();
		}

		/// <summary>
		///     Retrieves the available protection plugins.
		/// </summary>
		/// <param name="context">The working context.</param>
		/// <param name="protections">The working list of protections.</param>
		/// <param name="packers">The working list of packers.</param>
		/// <param name="components">The working list of components.</param>
		protected virtual void GetPluginsInternal(
			ConfuserContext context, IList<Protection> protections,
			IList<Packer> packers, IList<ConfuserComponent> components) {
			protections.Add(new WatermarkingProtection());
			try {
				Assembly protAsm = Assembly.Load("Confuser.Protections");
				AddPlugins(context, protections, packers, components, protAsm);
			}
			catch (Exception ex) {
				context.Logger.WarnException(Resources.PluginDiscovery_GetPluginsInternal_Failed_to_load_built_in_protections, ex);
			}

			try {
				Assembly renameAsm = Assembly.Load("Confuser.Renamer");
				AddPlugins(context, protections, packers, components, renameAsm);
			}
			catch (Exception ex) {
				context.Logger.WarnException(Resources.PluginDiscovery_GetPluginsInternal_Failed_to_load_renamer, ex);
			}

			try {
				Assembly renameAsm = Assembly.Load("Confuser.DynCipher");
				AddPlugins(context, protections, packers, components, renameAsm);
			}
			catch (Exception ex) {
				context.Logger.WarnException(Resources.PluginDiscovery_GetPluginsInternal_Failed_to_load_dynamic_cipher_library, ex);
			}

			#region load custom plugin
			plugModule.Clear();
			var paths = new List<string>();
			if (Directory.Exists(basePlugInsDir)) {
				var dlls = Directory.GetFiles(basePlugInsDir, "*.dll");
				paths.AddRange(dlls);
			}
			paths.AddRange(context.Project.PluginPaths);
			foreach (string pluginPath in paths) {
				string realPath = Path.Combine(context.BaseDirectory, pluginPath);
				try {
					Assembly plugin = Assembly.LoadFile(realPath);
					AddPlugins(context, protections, packers, components, plugin);
					plugModule.Add(ModuleDefMD.Load(realPath, new ModuleCreationOptions() { TryToLoadPdbFromDisk = true }));
				}
				catch (Exception ex) {
					context.Logger.WarnException(string.Format(Resources.PluginDiscovery_GetPluginsInternal_Failed_to_load_plugin, pluginPath), ex);
				}
			}
			#endregion
		}

		private List<ModuleDef> plugModule = new List<ModuleDef>();
		/// <summary>
		/// 获取插件模块
		/// </summary>
		/// <returns></returns>
		public List<ModuleDef> GetPluginModuleDef() {
			return plugModule;
		}
	}
}
