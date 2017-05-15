using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Diagnostics;
using FDK;

using SlimDXKey = SlimDX.DirectInput.Key;

namespace DTXMania
{
	public delegate void VoidFunc();

	/// <summary>
	/// Option 基底クラス。
	/// 注意：値の参照は Value プロパティを使ってください。
	/// 一部のクラスは暗黙的な型変換を実装していますが、
	/// オプションクラスインスタンス同士の等価比較には気をつけてください。
	/// </summary>
	[DataContract]
	public abstract class COptionBase
	{
		public EOptionType type;

		private string _label;
		/// <summary>
		/// コンフィグ画面におけるこの項目を表す文字列。
		/// nullの場合は、代わりにstrnameで名づけられたリソースから文字列を取得します。
		/// </summary>
		public string label
		{
			get
			{
				return (_label == null)? CDTXMania.Instance.Resources.Label(_strname ) : _label;
			}
			set
			{
				_label = value;
				_strname = null;
			}
		}

		private string _explanation;
		/// <summary>
		/// 説明。
		/// nullの場合は、代わりにstrnameで名づけられたリソースから文字列を取得します。
		/// </summary>
		public string explanation
		{
			get
			{
				return ( _explanation == null ) ? CDTXMania.Instance.Resources.Explanation( _strname ) : _explanation;
			}
			set
			{
				_explanation = value;
				_strname = null;
			}
		}

		private string _strname;
		/// <summary>
		/// 文字列の名前。リソースから文字列を取得する場合のkey文字列として使用。
		/// これがnullの場合は、labelとexplanationに設定された文字列を直接使います。
		/// </summary>
		public string strname
		{
			get
			{
				return _strname;
			}
			set
			{
				_strname = value;
			}
		
		}

		/// <summary>
		/// OnEnter で用いる動作。たとえば、この値によってほかのオプションに制約を与える場合などに
		/// その処理を登録しておけば、値の確定後実行されます。
		/// </summary>
		public VoidFunc OnEnterDelegate;

		/// <summary>
		/// 決定動作。OnEnterDelegate メンバに動作を登録します。
		/// </summary>
		public void OnEnter()
		{
			if (OnEnterDelegate != null)
			{
				OnEnterDelegate();
			}
		}

		/// <summary>
		/// 前の項目値動作。順序付け可能な値からなるオプションは相応に実装する必要があります。
		/// </summary>
		public virtual void OnPrevious()
		{
		}

		/// <summary>
		/// アイテムインデックス。
		/// </summary>
		public virtual int Index
		{
			get
			{
				return 0;
			}
			set
			{

			}
		}

		/// <summary>
		/// OnPrevious の逆順序用。
		/// </summary>
		public virtual void OnNext()
		{
		}
	}

	/// <summary>
	/// Option 基底クラス。
	/// DataContract により XML へのシリアライズ、XML からのデシリアライズが可能です。
	/// </summary>
	/// <typeparam name="T">このオプションの型。</typeparam>
	[DataContract]
	public abstract class COption<T> : COptionBase
	{
		[DataMember]
		protected T val;

		public virtual T Value
		{
			get
			{
				return val;
			}
			set
			{
				val = value;
			}
		}
	}

	[DataContract]
	public class COptionBool : COption<bool>
	{
		public static implicit operator bool (COptionBool b)
		{
			return b.Value;
		}

		public override int Index
		{
			get
			{
				return val ? 1 : 0;
			}
		}

		public COptionBool(bool init)
		{
			val = init;
		}

		public override void OnNext()
		{
			OnPrevious();
		}

		public override void OnPrevious()
		{
			val = !val;
		}

		public override string ToString()
		{
			CResources cr = CDTXMania.Instance.Resources;
			return val ? cr.Label("strCfgOn") : cr.Label("strCfgOff");
		}

		public void Initialize(string lbl, string expl)
		{
			label = lbl;
			explanation = expl;
			strname = null;
		}

		public void Initialize( string strname_ )
		{
			label = null;
			explanation = null;
			strname = strname_;
		}
	}

	[DataContract]
	public class COptionInteger : COption<int>
	{
		public int nStep;
		private int nMin;
		private int nMax;

		public static implicit operator int (COptionInteger n)
		{
			return n.Value;
		}

		public override int Index
		{
			get
			{
				return val;
			}
		}

		public COptionInteger(int init)
		{
			nMin = int.MinValue;
			nMax = int.MaxValue;
			val = init;
		}

		public override int Value
		{
			get
			{
				return base.Value;
			}

			set
			{
				base.Value = value;
				LimitValue();
			}
		}

		private void LimitValue()
		{
			if (val >= nMax)
			{
				val = nMax - 1;
			}
			if (val < nMin)
			{
				val = nMin;
			}
		}

		public override void OnNext()
		{
			IInputDevice keyboard = CDTXMania.Instance.Input管理.Keyboard;
			if ( keyboard.bキーが押されている( (int) SlimDXKey.LeftControl ) || keyboard.bキーが押されている( (int) SlimDXKey.RightControl ) )
			{
				val+=nStep;
			}
			else
			{
				val++;
			}
			LimitValue();
		}

		public override void OnPrevious()
		{
			IInputDevice keyboard = CDTXMania.Instance.Input管理.Keyboard;
			if ( keyboard.bキーが押されている( (int) SlimDXKey.LeftControl ) || keyboard.bキーが押されている( (int) SlimDXKey.RightControl ) )
			{
				val-=nStep;
			}
			else
			{
				val--;
			}
			LimitValue();
		}

		public Converter<int, string> ValueFormatter;

		public override string ToString()
		{
			string ret = "";
			if (ValueFormatter != null)
			{
				ret = ValueFormatter(val);
			}
			else
			{
				ret = val.ToString();
			}
			return ret;
		}

		public void Initialize(string lbl, string expl, int min = int.MinValue, int max = int.MaxValue, int step = 1)
		{
			label = lbl;
			explanation = expl;
			nMin = min;
			nMax = max;
			nStep = step;
			strname = null;
		}
		public void Initialize( string _strname, int min = int.MinValue, int max = int.MaxValue, int step = 1 )
		{
			label = null;
			explanation = null;
			nMin = min;
			nMax = max;
			nStep = step;
			strname = _strname;
		}
	}

	public interface IOptionList
	{

	}

	[DataContract]
	public abstract class COptionList<T> : COption<T>, IOptionList
	{
		protected T[] vals;
		protected int ptr;
		public int Length { get; protected set; }

		public T this[int idx]
		{
			get
			{
				return vals[idx];
			}
		}

		public override T Value
		{
			get
			{
				return base.Value;
			}

			set
			{
				base.Value = value;
				for (int i = 0; i < Length; ++i)
				{
					if (vals[i].Equals(value))
					{
						ptr = i;
					}
				}
			}
		}

		public override int Index
		{
			get
			{
				return ptr;
			}
			set
			{
				ptr = value;
				if (ptr >= Length) ptr = Length - 1;
				if (ptr < 0) ptr = 0;
				val = vals[ptr];
			}
		}

		public override void OnNext()
		{
			++ptr;
			if (ptr >= Length)
			{
				ptr = 0;
			}
			val = vals[ptr];
		}

		public override void OnPrevious()
		{
			--ptr;
			if (ptr < 0)
			{
				ptr = Length - 1;
			}
			val = vals[ptr];
		}

		public override string ToString()
		{
			return val.ToString();
		}

	}


	/// <summary>
	/// 列挙定数用オプションアイテム。
	/// </summary>
	/// <typeparam name="E">
	/// 列挙型。ただし、列挙型以外を渡してもコンパイル時エラーとならないので注意してください。(where 制約不可)
	/// また E のメンバに負の値を持つメンバを入れると意図した順番になりません。</typeparam>
	[DataContract]
	public class COptionEnum<E> : COptionList<E> where E : struct
	{
		public static implicit operator E(COptionEnum<E> e)
		{
			return e.Value;
		}

		/// <summary>
		/// リフレクションから列挙定数の集合を取得し、内部ポインタでまわします。
		/// ※実行時型情報の扱いは基本的に重いので注意してください。
		/// </summary>
		public COptionEnum(E init)
		{
			val = init;
		}

		public void Initialize(string lbl, string expl, Type type)
		{
			label = lbl;
			explanation = expl;
			strname = null;
			InitialieMain( type );
		}
		public void Initialize( string _strname, Type type )
		{
			label = null;
			explanation = null;
			strname = _strname;
			InitialieMain( type );
		}
		private void InitialieMain( Type type )
		{
			Length = type.GetEnumValues().Length;
			vals = new E[Length];
			int initptr = -1;
			ptr = 0;
			foreach (E t in typeof(E).GetEnumValues())
			{
				if (t.Equals(val))
				{
					initptr = ptr;
				}
				vals[ptr] = t;
				++ptr;
			}
			if (initptr == -1)
			{
				val = vals[0];
				ptr = 0;
			}
			else
			{
				ptr = initptr;
			}
		}

	}

	[DataContract]
	public class COptionString : COption<string>
	{
		public static implicit operator string (COptionString s)
		{
			return s.Value;
		}

		public COptionString(string init)
		{
			val = init;
		}

		public void Initialize(string lbl, string expl)
		{
			label = lbl;
			explanation = expl;
			strname = null;
		}
		public void Initialize( string _strname )
		{
			label = null;
			explanation = null;
			strname = _strname;
		}

		public override string ToString()
		{
			return val;
		}
	}

	[DataContract]
	public class COptionStringList : COptionList<string>
	{
		public COptionStringList(string init)
		{
			val = init;
		}

		public void Initialize(string lbl, string expl, string[] initvals)
		{
			label = lbl;
			explanation = expl;
			strname = null;
			InitializeMain( initvals );
		}
		public void Initialize( string _strname, string[] initvals )
		{
			label = null;
			explanation = null;
			strname = _strname;
			InitializeMain( initvals );
		}
		public void InitializeMain( string[] initvals )
		{
			Length = initvals.Length;
			vals = new string[Length];
			int initptr = -1;
			ptr = 0;
			foreach (var t in initvals)
			{
				if (t == val)
				{
					initptr = ptr;
				}
				vals[ptr] = t;
				++ptr;
			}

			if (initptr == -1)
			{
				val = vals[0];
				ptr = 0;
			}
			else
			{
				ptr = initptr;
			}
		}
	}

	[DataContract]
	public class COptionFloat : COption<float>
	{
		public static implicit operator float (COptionFloat f)
		{
			return f.Value;
		}

		public COptionFloat(float init)
		{
			val = init;
		}

		public void Initialize( string lbl, string expl )
		{
			label = lbl;
			explanation = expl;
			strname = null;
		}
		public void Initialize( string _strname )
		{
			label = null;
			explanation = null;
			strname = _strname;
		}

		public override string ToString()
		{
			return val.ToString();
		}
	}

	public class COptionLabel : COptionString
	{
		public COptionLabel(string lbl, string expl)
			: base("")
		{
			type = EOptionType.Other;
			label = lbl;
			explanation = expl;
			strname = null;
		}
		public COptionLabel( string _strname )
			: base( "" )
		{
			type = EOptionType.Other;
			label = null;
			explanation = null;
			strname = _strname;
		}
	}

	[DataContract]
	public class COptionDictionary<K, V> : COption<Dictionary<K, V>>
	{
		public V this[K t]
		{
			get
			{
				return Value[t];
			}
		}
	}

	[DataContract]
	public class COptionKeyAssign : COption<CKeyAssign>
	{
		public EInputDevice 入力デバイス
		{
			get
			{
				return Value.InputDevice;
			}
			set
			{
				Value.InputDevice = value;
			}
		}

		public int ID
		{
			get
			{
				return Value.ID;
			}
			set
			{
				Value.ID = value;
			}
		}

		public int コード
		{
			get
			{
				return Value.Code;
			}
			set
			{
				Value.Code = value;
			}
		}

		public COptionKeyAssign()
		{
			val = new CKeyAssign(EInputDevice.Unknown, 0, 0);
		}

		public void Reset()
		{
			Value.InputDevice = EInputDevice.Unknown;
			Value.ID = 0;
			Value.Code = 0;
		}

		public void CopyFrom(CKeyAssign t)
		{
			Value.InputDevice = t.InputDevice;
			Value.ID = t.ID;
			Value.Code = t.Code;
		}
	}

	[DataContract]
	public class COptionPadBool : STPadValue<COptionBool>
	{
		private bool DetermineAssignValue(EThreeState state)
		{
			return state == EThreeState.On ? true : state == EThreeState.Off ? false : false;
		}

		public bool bIsAutoHH
		{
			get
			{
				return !LC && HH && !SD && !BD && !HT && !LT && !FT && !CY && !RD && HHO;
			}
		}

		public bool bIsAutoBD
		{
			get
			{
				return !LC && !HH && !SD && BD && !HT && !LT && !FT && !CY && !RD && !HHO;
			}
		}

		public bool bIsAutoNeck(EPart e)
		{
			bool ret = false;
			if (e == EPart.Guitar)
			{
				ret = GtR && GtG && GtB && !GtPick;
			}
			else if (e == EPart.Bass)
			{
				ret = BsR && BsG && BsB && !BsPick;
			}
			return ret;
		}

		public bool bIsAutoPick(EPart e)
		{
			bool ret = false;
			if (e == EPart.Guitar)
			{
				ret = !GtR && !GtG && !GtB && GtPick;
			}
			else if (e == EPart.Bass)
			{
				ret = !BsR && !BsG && !BsB && BsPick;
			}
			return ret;
		}

		public void SetAutoHH()
		{
			Set(EPart.Drums, EThreeState.Off);
			HH.Value = true;
		}

		public void SetAutoBD()
		{
			Set(EPart.Drums, EThreeState.Off);
			BD.Value = true;
		}

		public void SetAutoPick(EPart e)
		{
			Set(e, EThreeState.Off);
			if (e == EPart.Drums)
			{
				GtPick.Value = true;
			}
			else if (e == EPart.Bass)
			{
				BsPick.Value = true;
			}
		}

		public void SetAutoNeck(EPart e)
		{
			Set(e, EThreeState.Off);
			if (e == EPart.Drums)
			{
				GtR.Value = true;
				GtG.Value = true;
				GtB.Value = true;
			}
			else if (e == EPart.Bass)
			{
				BsR.Value = true;
				BsG.Value = true;
				BsB.Value = true;
			}
		}

		public void Set(EPart e, EThreeState state)
		{
			bool val = DetermineAssignValue(state);
			if (EThreeState.X != state)
			{
				if (e == EPart.Drums)
				{
					LC.Value = HH.Value = HHO.Value = SD.Value =
							BD.Value = HT.Value = LT.Value = FT.Value = CY.Value = RD.Value = val;
				}
				else if (e == EPart.Guitar)
				{
					GtR.Value = GtG.Value = GtB.Value = GtPick.Value = GtWail.Value = val;
				}
				else if (e == EPart.Bass)
				{
					BsR.Value = BsG.Value = BsB.Value = BsPick.Value = BsWail.Value = val;
				}
			}
		}

		public bool IsAllFalse(EPart x)
		{
			bool ret = false;
			if (x == EPart.Guitar)
			{
				ret = !GtR && !GtG && !GtB && !GtPick && !GtWail;
			}
			else if (x == EPart.Bass)
			{
				ret = !BsR && !BsG && !BsB && !BsPick && !BsWail;
			}
			else if (x == EPart.Drums)
			{
				ret = !LC && !HH && !SD && !BD && !HT && !LT && !FT && !CY && !RD && !HHO;
			}
			return ret;
		}

		public bool IsAllTrue(EPart x)
		{
			bool ret = false;
			if (x == EPart.Guitar)
			{
				ret = GtR && GtG && GtB && GtPick && GtWail;
			}
			else if (x == EPart.Bass)
			{
				ret = BsR && BsG && BsB && BsPick && BsWail;
			}
			else if (x == EPart.Drums)
			{
				ret = LC && HH && SD && BD && HT && LT && FT && CY && RD && HHO;
			}
			return ret;
		}
	}

}
