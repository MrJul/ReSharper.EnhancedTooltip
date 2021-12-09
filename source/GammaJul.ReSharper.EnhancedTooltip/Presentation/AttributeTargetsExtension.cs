using System;
using System.Text;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	internal static class AttributeTargetsExtension {

		public static string ToHumanReadableString(this AttributeTargets targets) {
			if (targets == 0)
				return "Nowhere";
			if (targets == AttributeTargets.All)
				return "Everywhere";
			
			var builder = new StringBuilder(150);
			AppendTarget(builder, targets, AttributeTargets.Assembly, "Assembly");
			AppendTarget(builder, targets, AttributeTargets.Module, "Module");
			AppendTarget(builder, targets, AttributeTargets.Class, "Class");
			AppendTarget(builder, targets, AttributeTargets.Struct, "Struct");
			AppendTarget(builder, targets, AttributeTargets.Enum, "Enum");
			AppendTarget(builder, targets, AttributeTargets.Constructor, "Constructor");
			AppendTarget(builder, targets, AttributeTargets.Method, "Method");
			AppendTarget(builder, targets, AttributeTargets.Property, "Property");
			AppendTarget(builder, targets, AttributeTargets.Field, "Field");
			AppendTarget(builder, targets, AttributeTargets.Event, "Event");
			AppendTarget(builder, targets, AttributeTargets.Interface, "Interface");
			AppendTarget(builder, targets, AttributeTargets.Parameter, "Parameter");
			AppendTarget(builder, targets, AttributeTargets.Delegate, "Delegate");
			AppendTarget(builder, targets, AttributeTargets.ReturnValue, "Return value");
			AppendTarget(builder, targets, AttributeTargets.GenericParameter, "Generic parameter");
			return builder.Length > 0 ? builder.ToString() : "Unknown";
		}

		private static void AppendTarget(StringBuilder builder, AttributeTargets currentTargets, AttributeTargets expectedTarget, string display) {
			if ((currentTargets & expectedTarget) == expectedTarget) {
				if (builder.Length > 0)
					builder.Append(", ");
				builder.Append(display);
			}
		}

	}

}