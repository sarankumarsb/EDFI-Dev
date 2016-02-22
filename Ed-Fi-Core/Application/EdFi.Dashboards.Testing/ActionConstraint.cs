// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Mocks.Constraints;

namespace EdFi.Dashboards.Testing
{
	public class ActionConstraint<T> : AbstractConstraint
	{
		private readonly Action<T> action;

		public ActionConstraint(Action<T> action)
		{
			this.action = action;
		}

		public override bool Eval(object obj)
		{
			action((T) obj);

			return true;
		}

		public override string Message
		{
			get { return "ActionConstraint fired."; }
		}
	}

    public class FuncConstraint<T> : AbstractConstraint
    {
        private readonly Func<T, bool> _func;

        public FuncConstraint(Func<T, bool> func)
        {
            this._func = func;
        }

        public override bool Eval(object obj)
        {
            return _func((T)obj);
        }

        public override string Message
        {
            get { return "FuncConstraint fired."; }
        }
    }
}