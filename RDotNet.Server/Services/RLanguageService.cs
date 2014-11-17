using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.ServiceModel;
using RDotNet.R.Adapter;
using RDotNet.R.Adapter.Graphics;

namespace RDotNet.Server.Services
{
    public class RLanguageService : IRLanguageService
    {
        private readonly RInstance _instance;

        public RLanguageService()
            : this(new RInstanceManager())
        {}

        public RLanguageService(IRInstanceManager instanceManager)
        {
            if (instanceManager == null) throw new ArgumentNullException("instanceManager");

            _instance = instanceManager.GetInstance();
        }

        public string GetRVersion()
        {
            const string msg = "sessionInfo()\n";
            var s = MakeString(msg);

            ParseStatus status;
            var vector = ParseVector(s, -1, out status);
            if (status != ParseStatus.Ok) throw new FaultException<ParseFault>(new ParseFault(), "Could not parse vector.");

            var expression = GetExpressionVectorValueAt(vector, 0);

            bool errorOccurred;
            var evaluated = TryEvaluate(expression, GetGlobalEnvironment(), out errorOccurred);
            if (errorOccurred) throw new FaultException<ParseFault>(new ParseFault(), "Could not evaluate expression.");

            var messages = CoerceVector(evaluated, SymbolicExpressionType.CharacterVector);
            var message = GetCharacterVectorValueAt(messages, 0);

            var start = message.IndexOf("R version", StringComparison.InvariantCultureIgnoreCase);
            var result = message.Substring(start);
            return result;
        }

        public void PreserveObject(SymbolicExpressionContext context)
        {
            _instance.GetFunction<R_PreserveObject>()(context.Handle);
        }

        [SecurityCritical]
        [HandleProcessCorruptedStateExceptions]
        public void ReleaseObject(SymbolicExpressionContext context)
        {
            try
            {
                _instance.GetFunction<R_ReleaseObject>()(context.Handle);
            }
            catch (NullReferenceException ex)
            {
                Debugger.Break();
            }
            catch (AccessViolationException)
            { }
        }

        public SymbolicExpressionContext Protect(SymbolicExpressionContext context)
        {
            Console.WriteLine("<--- {0}", context.Handle);
            var handle = _instance.GetFunction<Rf_protect>()(context.Handle);
            return new SymbolicExpressionContext(handle);
        }

        public void Unprotect(int count)
        {
            _instance.GetFunction<Rf_unprotect>()(count);
        }

        public void UnprotectPtr(SymbolicExpressionContext context)
        {
            Console.WriteLine("---> {0}", context.Handle);
            _instance.GetFunction<Rf_unprotect_ptr>()(context.Handle);
        }

        public SymbolicExpressionContext Install(string s)
        {
            var result = _instance.GetFunction<Rf_install>()(s);
            return new SymbolicExpressionContext(result);
        }

        public SymbolicExpressionContext MakeString(string s)
        {
            var handle = _instance.GetFunction<Rf_mkString>()(s);
            return new SymbolicExpressionContext(handle);
        }

        public SymbolicExpressionContext MakeChar(string s)
        {
            var handle = _instance.GetFunction<Rf_mkChar>()(s);
            return new SymbolicExpressionContext(handle);
        }

        public SymbolicExpressionContext AsCharacterFactor(SymbolicExpressionContext context)
        {
            var factor = _instance.GetFunction<Rf_asCharacterFactor>()(context.Handle);
            return new SymbolicExpressionContext(factor);
        }

        public SymbolicExpressionContext AllocateVector(SymbolicExpressionType type, int length)
        {
            var vector = _instance.GetFunction<Rf_allocVector>()(type, length);
            return new SymbolicExpressionContext(vector);
        }

        public SymbolicExpressionContext CoerceVector(SymbolicExpressionContext context, SymbolicExpressionType type)
        {
            var vector = _instance.GetFunction<Rf_coerceVector>()(context.Handle, type);
            return new SymbolicExpressionContext(vector);
        }

        public bool IsVector(SymbolicExpressionContext context)
        {
            var result = _instance.GetFunction<Rf_isVector>()(context.Handle);
            return result;
        }

        public bool IsFrame(SymbolicExpressionContext context)
        {
            var result = _instance.GetFunction<Rf_isFrame>()(context.Handle);
            return result;
        }

        public bool IsS4(SymbolicExpressionContext context)
        {
            var result = _instance.GetFunction<Rf_isS4>()(context.Handle);
            return result;
        }

        public int GetLength(SymbolicExpressionContext context)
        {
            var length = _instance.GetFunction<Rf_length>()(context.Handle);
            return length;
        }

        public SymbolicExpressionContext AllocateMatrix(SymbolicExpressionType type, int rowCount, int columnCount)
        {
            var matrix = _instance.GetFunction<Rf_allocMatrix>()(type, rowCount, columnCount);
            return new SymbolicExpressionContext(matrix);
        }

        public bool IsMatrix(SymbolicExpressionContext context)
        {
            var result = _instance.GetFunction<Rf_isMatrix>()(context.Handle);
            return result;
        }

        public bool IsPromise(SymbolicExpressionContext context)
        {
            return context.RawSEXP.sxpinfo.type == SymbolicExpressionType.Promise;
        }

        //TODO: Find the appropriate RInternals calls for these.
        public SymbolicExpressionContext GetClosureBody(SymbolicExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public SymbolicExpressionContext GetClosureFormals(SymbolicExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public SymbolicExpressionContext GetClosureEnvironment(SymbolicExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public SymbolicExpressionContext GetLanguageList(SymbolicExpressionContext context)
        {
            throw new NotImplementedException();
        }

        //TODO: Is this a dupe?
        public SymbolicExpressionContext GetClass(SymbolicExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public int NumberOfRows(SymbolicExpressionContext context)
        {
            var rows = _instance.GetFunction<Rf_nrows>()(context.Handle);
            return rows;
        }

        public int NumberOfColumns(SymbolicExpressionContext context)
        {
            var columns = _instance.GetFunction<Rf_ncols>()(context.Handle);
            return columns;
        }

        public SymbolicExpressionContext AllocateList(int length)
        {
            var list = _instance.GetFunction<Rf_allocList>()(length);
            return new SymbolicExpressionContext(list);
        }

        public bool IsList(SymbolicExpressionContext context)
        {
            //Rf_isList in the R API is NOT the correct thing to use (yes, hard to be more conter-intuitive)
            return (context.Type == SymbolicExpressionType.List ||
                    (context.Type == SymbolicExpressionType.Pairlist && GetLength(context) > 0));

        }

        [SecurityCritical]
        [HandleProcessCorruptedStateExceptions]
        public SymbolicExpressionContext Evaluate(SymbolicExpressionContext statement,
                                                  SymbolicExpressionContext environment)
        {
            try
            {
                var result = _instance.GetFunction<Rf_eval>()(statement.Handle, environment.Handle);
                if (result != IntPtr.Zero && GetVisible())
                {
                    _instance.GetFunction<Rf_PrintValue>()(result);
                }
                return new SymbolicExpressionContext(result);
            }
            catch (PlotException ex)
            {
                throw new FaultException<EvaluationFault>(new EvaluationFault(), ex.InnerException.Message);
            }
            catch (AccessViolationException)
            {
                throw new FaultException<EvaluationFault>(new EvaluationFault(), "Critical failure during R execution.");
            }
        }

        [SecurityCritical]  
        [HandleProcessCorruptedStateExceptions]
        public SymbolicExpressionContext TryEvaluate(SymbolicExpressionContext statement,
                                                     SymbolicExpressionContext environment,
                                                     out bool errorOccurred)
        {
            try
            {
                Console.WriteLine("<- Statement: {0} Environment: {1} ->", statement.Handle, environment.Handle);
                var result = _instance.GetFunction<R_tryEval>()(statement.Handle, environment.Handle, out errorOccurred);
                if (result != IntPtr.Zero && GetVisible())
                {
                    _instance.GetFunction<Rf_PrintValue>()(result);
                }
                return !errorOccurred ? new SymbolicExpressionContext(result) : null;
            }
            catch (PlotException ex)
            {
                throw new FaultException<EvaluationFault>(new EvaluationFault(), ex.InnerException.Message);
            }
            catch (AccessViolationException)
            {
                throw new FaultException<EvaluationFault>(new EvaluationFault(), "Critical failure during R execution.");
            }
        }

        public SymbolicExpressionContext ParseVector(SymbolicExpressionContext statement,
                                                     int statementCount,
                                                     out ParseStatus status)
        {
            var vector = _instance.GetFunction<R_ParseVector>()(statement.Handle, -1, out status, GetNilValue().Handle);
            return new SymbolicExpressionContext(vector);
        }

        public SymbolicExpressionContext FindVar(SymbolicExpressionContext name, SymbolicExpressionContext environment)
        {
            var result = _instance.GetFunction<Rf_findVar>()(name.Handle, environment.Handle);
            return new SymbolicExpressionContext(result);
        }

        public void SetVar(SymbolicExpressionContext name,
                           SymbolicExpressionContext value,
                           SymbolicExpressionContext environment)
        {
            _instance.GetFunction<Rf_setVar>()(name.Handle, value.Handle, environment.Handle);
        }

        public void DefineVar(SymbolicExpressionContext name,
                              SymbolicExpressionContext value,
                              SymbolicExpressionContext environment)
        {
            _instance.GetFunction<Rf_defineVar>()(name.Handle, value.Handle, environment.Handle);
        }

        public SymbolicExpressionContext GetAttrib(SymbolicExpressionContext context, SymbolicExpressionContext name)
        {
            var attribute = _instance.GetFunction<Rf_getAttrib>()(context.Handle, name.Handle);
            return new SymbolicExpressionContext(attribute);
        }

        public SymbolicExpressionContext SetAttrib(SymbolicExpressionContext context, SymbolicExpressionContext name,
            SymbolicExpressionContext value)
        {
            var result = _instance.GetFunction<Rf_setAttrib>()(context.Handle, name.Handle, value.Handle);
            return new SymbolicExpressionContext(result);
        }

        public SymbolicExpressionContext DoSlot(SymbolicExpressionContext context, SymbolicExpressionContext name)
        {
            var slot = _instance.GetFunction<R_do_slot>()(context.Handle, name.Handle);
            return new SymbolicExpressionContext(slot);
        }

        public SymbolicExpressionContext DoSlotAssign(SymbolicExpressionContext context, SymbolicExpressionContext name,
            SymbolicExpressionContext value)
        {
            var slot = _instance.GetFunction<R_do_slot_assign>()(context.Handle, name.Handle, value.Handle);
            return new SymbolicExpressionContext(slot);
        }

        public SymbolicExpressionContext GetClassDefinition(string what)
        {
            var classDefinition = _instance.GetFunction<R_getClassDef>()(what);
            return new SymbolicExpressionContext(classDefinition);
        }

        public bool HasSlot(SymbolicExpressionContext context, SymbolicExpressionContext name)
        {
            var result = _instance.GetFunction<R_has_slot>()(context.Handle, name.Handle);
            return result;
        }

        public bool IsEnvironment(SymbolicExpressionContext context)
        {
            var result = _instance.GetFunction<Rf_isEnvironment>()(context.Handle);
            return result;
        }

        public bool IsExpression(SymbolicExpressionContext context)
        {
            var result = _instance.GetFunction<Rf_isExpression>()(context.Handle);
            return result;
        }

        public bool IsSymbol(SymbolicExpressionContext context)
        {
            var result = _instance.GetFunction<Rf_isSymbol>()(context.Handle);
            return result;
        }

        public bool IsLanguage(SymbolicExpressionContext context)
        {
            var result = _instance.GetFunction<Rf_isLanguage>()(context.Handle);
            return result;
        }

        public bool IsFunction(SymbolicExpressionContext context)
        {
            var result = _instance.GetFunction<Rf_isFunction>()(context.Handle);
            return result;
        }

        public bool IsFactor(SymbolicExpressionContext context)
        {
            var result = _instance.GetFunction<Rf_isFactor>()(context.Handle);
            return result;
        }

        public bool IsOrdered(SymbolicExpressionContext context)
        {
            var result = _instance.GetFunction<Rf_isOrdered>()(context.Handle);
            return result;
        }

        public SymbolicExpressionContext LsInternal(SymbolicExpressionContext environment, bool all)
        {
            var result = _instance.GetFunction<R_lsInternal>()(environment.Handle, all);
            return new SymbolicExpressionContext(result);
        }

        public SymbolicExpressionContext ApplyClosure(SymbolicExpressionContext call,
                                                      SymbolicExpressionContext value,
                                                      SymbolicExpressionContext arguments,
                                                      SymbolicExpressionContext environment,
                                                      SymbolicExpressionContext suppliedEnvironment)
        {
            var result = _instance.GetFunction<Rf_applyClosure>()(call.Handle,
                                                             value.Handle,
                                                             arguments.Handle,
                                                             environment.Handle,
                                                             suppliedEnvironment.Handle);

            return new SymbolicExpressionContext(result);
        }

        public SymbolicExpressionContext VectorToPairList(SymbolicExpressionContext context)
        {
            var result = _instance.GetFunction<Rf_VectorToPairList>()(context.Handle);
            return new SymbolicExpressionContext(result);
        }

        public SymbolicExpressionContext AllocateSEXP(SymbolicExpressionType type)
        {
            var context = _instance.GetFunction<Rf_allocSExp>()(type);
            return new SymbolicExpressionContext(context);
        }

        public SymbolicExpressionContext NewEnvironment(SymbolicExpressionContext names,
                                                        SymbolicExpressionContext values,
                                                        SymbolicExpressionContext parent)
        {
            var result = _instance.GetFunction<Rf_NewEnvironment>()(names.Handle, values.Handle, parent.Handle);
            return new SymbolicExpressionContext(result);
        }

        public SymbolicExpressionContext Cons(SymbolicExpressionContext context, SymbolicExpressionContext next)
        {
            var cons = _instance.GetFunction<Rf_cons>()(context.Handle, next.Handle);
            return new SymbolicExpressionContext(cons);
        }

        public SymbolicExpressionContext LCons(SymbolicExpressionContext context, SymbolicExpressionContext next)
        {
            var lcons = _instance.GetFunction<Rf_lcons>()(context.Handle, next.Handle);
            return new SymbolicExpressionContext(lcons);
        }

        public bool IsNil(SymbolicExpressionContext context)
        {
            var symbol = GetNilValue();
            return symbol.Handle == context.Handle;
        }

        public SymbolicExpressionContext GetNames()
        {
            var symbol = GetPredefinedSymbol("R_NamesSymbol");
            return new SymbolicExpressionContext(symbol);
        }

        public List<string> GetCharacterVectorValues(SymbolicExpressionContext context)
        {
            var length = GetLength(context);
            var values = new List<string>(length);
            for (var i = 0; i < length; i++)
            {
                var value = GetCharacterVectorValueAt(context, i);
                values.Add(value);
            }

            return values;
        }

        public List<SymbolicExpressionContext> GetExpressionVectorValues(SymbolicExpressionContext context)
        {
            var length = GetLength(context);
            var values = new List<SymbolicExpressionContext>(length);
            for (var i = 0; i < length; i++)
            {
                var value = GetExpressionVectorValueAt(context, i);
                values.Add(value);
            }

            return values;
        }

        public List<int> GetIntegerVectorValues(SymbolicExpressionContext context)
        {
            var length = GetLength(context);
            var values = new int[length];
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            Marshal.Copy(dataPointer, values, 0, length);

            return values.ToList();
        }

        public List<double> GetNumericVectorValues(SymbolicExpressionContext context)
        {
            var length = GetLength(context);
            var values = new double[length];
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            Marshal.Copy(dataPointer, values, 0, length);

            return values.ToList();
        }

        public List<bool> GetLogicalVectorValues(SymbolicExpressionContext context)
        {
            var length = GetLength(context);
            var values = new int[length];
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            Marshal.Copy(dataPointer, values, 0, length);

            return Array.ConvertAll(values, Convert.ToBoolean).ToList();
        }

        public List<byte> GetRawVectorValues(SymbolicExpressionContext context)
        {
            var length = GetLength(context);
            var values = new byte[length];
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            Marshal.Copy(dataPointer, values, 0, length);

            return values.ToList();
        }

        public List<Complex> GetComplexVectorValues(SymbolicExpressionContext context)
        {
            var length = GetLength(context);
            var values = new double[length*2];
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            Marshal.Copy(dataPointer, values, 0, length*2);

            var result = new List<Complex>(length);
            for (var i = 0; i < length*2; i += 2)
            {
                result.Add(new Complex(values[i], values[i + 1]));
            }

            return result;
        }

        public SymbolicExpressionContext GetDimNames()
        {
            var symbol = GetPredefinedSymbol("R_DimNamesSymbol");
            return new SymbolicExpressionContext(symbol);
        }

        public SymbolicExpressionContext GetUnboundValue()
        {
            var symbol = GetPredefinedSymbol("R_UnboundValue");
            return new SymbolicExpressionContext(symbol);
        }

        public SymbolicExpressionContext GetNilValue()
        {
            var symbol = GetPredefinedSymbol("R_NilValue");
            return new SymbolicExpressionContext(symbol);
        }

        public SymbolicExpressionContext GetGlobalEnvironment()
        {
            var symbol = GetPredefinedSymbol("R_GlobalEnv");
            return new SymbolicExpressionContext(symbol);
        }

        public bool GetVisible()
        {
            var symbol = _instance.GetPointer("R_Visible");
            var value = Marshal.ReadInt32(symbol);
            var result = Convert.ToBoolean(value);
            return result;
        }

        public List<SymbolicExpressionContext> GetSymbols(SymbolicExpressionContext context)
        {
            var values = new List<SymbolicExpressionContext>();
            SEXPREC sexp = context.RawSEXP;
            while (sexp.sxpinfo.type != SymbolicExpressionType.Null)
            {
                values.Add(new SymbolicExpressionContext(sexp.listsxp.tagval));
                sexp = (SEXPREC) Marshal.PtrToStructure(sexp.listsxp.cdrval, typeof(SEXPREC));
            }

            return values;
        }

        public bool IsNaString(SymbolicExpressionContext context)
        {
            var symbol = GetNAString();
            return symbol == context.Handle;
        }

        public string GetParseError()
        {
            var pointer = _instance.GetPointer("R_ParseErrorMsg");
            var message = Marshal.PtrToStringAnsi(pointer);

            pointer = _instance.GetPointer("R_ParseError");
            var lineNumber = Marshal.ReadInt32(pointer);

            var result = string.Format("Line: {0} Error: {1}", lineNumber, message);
            return result;
        }

        public List<string> GetAttributeNames(SymbolicExpressionContext context)
        {
            int length = GetLength(new SymbolicExpressionContext(context.RawSEXP.attrib));
            var pointer = context.RawSEXP.attrib;
            var names = new List<string>(length);
            for (var index = 0; index < length; index++)
            {
                var node = (SEXPREC) Marshal.PtrToStructure(pointer, typeof(SEXPREC));
                var attribute = (SEXPREC) Marshal.PtrToStructure(node.listsxp.tagval, typeof(SEXPREC));
                var stringPointer = IntPtr.Add(attribute.symsxp.pname, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
                var name = Marshal.PtrToStringAnsi(stringPointer);
                names.Add(name);
                pointer = node.listsxp.cdrval;
            }

            return names;
        }

        public SymbolicExpressionContext GetExpressionVectorValueAt(SymbolicExpressionContext context, int index)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            int dataSize = Marshal.SizeOf(typeof(IntPtr));
            var itemPointer = Marshal.ReadIntPtr(dataPointer, index*dataSize);

            var result = new SymbolicExpressionContext(itemPointer);
            return result;
        }

        public void SetExpressionVectorValueAt(SymbolicExpressionContext context,
                                               int index,
                                               SymbolicExpressionContext value)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            int dataSize = Marshal.SizeOf(typeof(IntPtr));

            Marshal.WriteIntPtr(dataPointer, index*dataSize, value.Handle);
        }

        public string GetCharacterVectorValueAt(SymbolicExpressionContext context, int index)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            int dataSize = Marshal.SizeOf(typeof(IntPtr));
            var itemPointer = Marshal.ReadIntPtr(dataPointer, index*dataSize);
            if (itemPointer == GetNAString()) return null;

            var itemDataPointer = IntPtr.Add(itemPointer, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            var result = Marshal.PtrToStringAnsi(itemDataPointer);
            return result;
        }

        public void SetCharacterVectorValueAt(SymbolicExpressionContext context, int index, string value)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            int dataSize = Marshal.SizeOf(typeof(IntPtr));
            IntPtr charPtr = (value == null) ? GetNAString() : MakeChar(value).Handle;
            Marshal.WriteIntPtr(dataPointer, index*dataSize, charPtr);
        }

        public int GetIntegerVectorValueAt(SymbolicExpressionContext context, int index)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));

            var result = Marshal.ReadInt32(dataPointer, index*sizeof(int));
            return result;
        }

        public void SetIntegerVectorValueAt(SymbolicExpressionContext context, int index, int value)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            Marshal.WriteInt32(dataPointer, index*sizeof(int), value);
        }

        public double GetNumericVectorValueAt(SymbolicExpressionContext context, int index)
        {
            var data = new double[1];
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            dataPointer = IntPtr.Add(dataPointer, index*sizeof(double));

            Marshal.Copy(dataPointer, data, 0, data.Length);

            return data[0];
        }

        public void SetNumericVectorValueAt(SymbolicExpressionContext context, int index, double value)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            dataPointer = IntPtr.Add(dataPointer, index*sizeof(double));

            Marshal.Copy(new[] {value}, 0, dataPointer, 1);
        }

        public bool GetLogicalVectorValueAt(SymbolicExpressionContext context, int index)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            var value = Marshal.ReadInt32(dataPointer, index*sizeof(int));

            var result = Convert.ToBoolean(value);
            return result;
        }

        public void SetLogicalVectorValueAt(SymbolicExpressionContext context, int index, bool value)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            var coerced = Convert.ToInt32(value);

            Marshal.WriteInt32(dataPointer, index*sizeof(int), coerced);
        }

        public byte GetRawVectorValueAt(SymbolicExpressionContext context, int index)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));

            var result = Marshal.ReadByte(dataPointer, index*sizeof(byte));
            return result;
        }

        public void SetRawVectorValueAt(SymbolicExpressionContext context, int index, byte value)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));

            Marshal.WriteByte(dataPointer, index*sizeof(byte), value);
        }

        public Complex GetComplexVectorValueAt(SymbolicExpressionContext context, int index)
        {
            var data = new double[2];
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            var itemPointer = IntPtr.Add(dataPointer, index*sizeof(double)*2);
            Marshal.Copy(itemPointer, data, 0, data.Length);

            var result = new Complex(data[0], data[1]);
            return result;
        }

        public void SetComplexVectorValueAt(SymbolicExpressionContext context, int index, Complex value)
        {
            var data = new[] {value.Real, value.Imaginary};
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            var itemPointer = IntPtr.Add(dataPointer, index*sizeof(double)*2);
            Marshal.Copy(data, 0, itemPointer, data.Length);
        }

        public void InitializeExpressionVector(SymbolicExpressionContext context, List<SymbolicExpressionContext> vector)
        {
            for (var i = 0; i < vector.Count; i++)
            {
                SetExpressionVectorValueAt(context, i, vector[i]);
            }
        }

        public void InitializeCharacterVector(SymbolicExpressionContext context, List<string> vector)
        {
            for (var i = 0; i < vector.Count; i++)
            {
                SetCharacterVectorValueAt(context, i, vector[i]);
            }
        }

        public void InitializeIntegerVector(SymbolicExpressionContext context, List<int> vector)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf((typeof(VECTOR_SEXPREC))));
            var coerced = vector.ToArray();
            Marshal.Copy(coerced, 0, dataPointer, coerced.Length);
        }

        public void InitializeNumericVector(SymbolicExpressionContext context, List<double> vector)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf((typeof(VECTOR_SEXPREC))));
            var coerced = vector.ToArray();
            Marshal.Copy(coerced, 0, dataPointer, coerced.Length);
        }

        public void InitializeLogicalVector(SymbolicExpressionContext context, List<bool> vector)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf((typeof(VECTOR_SEXPREC))));
            var coerced = vector.Select(Convert.ToInt32).ToArray();
            Marshal.Copy(coerced, 0, dataPointer, coerced.Length);
        }

        public void InitializeRawVector(SymbolicExpressionContext context, List<byte> vector)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf((typeof(VECTOR_SEXPREC))));
            var coerced = vector.ToArray();
            Marshal.Copy(coerced, 0, dataPointer, coerced.Length);
        }

        public void InitializeComplexVector(SymbolicExpressionContext context, List<Complex> vector)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf((typeof(VECTOR_SEXPREC))));
            var coerced = ConvertComplexToDouble(vector);
            Marshal.Copy(coerced, 0, dataPointer, coerced.Length);
        }

        public string GetPrintName(SymbolicExpressionContext context)
        {
            var strPtr = IntPtr.Add(context.RawSEXP.symsxp.pname, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            return Marshal.PtrToStringAnsi(strPtr);
        }

        public SymbolicExpressionContext GetLevels(SymbolicExpressionContext context)
        {
            var result = GetPredefinedSymbol("R_LevelsSymbol");
            return new SymbolicExpressionContext(result);
        }

        public List<List<double>> GetNumericMatrixValues(SymbolicExpressionContext context)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            var columnCount = GetColumnCount(context);
            var rowCount = GetRowCount(context);
            var length = columnCount * rowCount;
            var values = new double[length];
            Marshal.Copy(dataPointer, values, 0, length);

            var result = UnflattenToMatrix(values, rowCount, columnCount);
            return result;
        }

        public List<List<int>> GetIntegerMatrixValues(SymbolicExpressionContext context)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            var columnCount = GetColumnCount(context);
            var rowCount = GetRowCount(context);
            var length = columnCount*rowCount;
            var values = new int[length];
            Marshal.Copy(dataPointer, values, 0, length);

            var result = UnflattenToMatrix(values, rowCount, columnCount);
            return result;
        }

        public List<List<string>> GetCharacterMatrixValues(SymbolicExpressionContext context)
        {
            var columnCount = GetColumnCount(context);
            var rowCount = GetRowCount(context);

            var result = new List<List<string>>(columnCount);
            for (var column = 0; column < columnCount; column++)
            {
                var cl = new List<string>();
                for (var row = 0; row < rowCount; row++)
                {
                    var value = GetCharacterMatrixValueAt(context, row, column);
                    cl.Add(value);
                }
                result.Add(cl);
            }

            return result;
        }

        public List<List<bool>> GetLogicalMatrixValues(SymbolicExpressionContext context)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            var columnCount = GetColumnCount(context);
            var rowCount = GetRowCount(context);
            var length = columnCount * rowCount;
            var values = new int[length];
            Marshal.Copy(dataPointer, values, 0, length);

            var result = UnflattenToMatrix(values, rowCount, columnCount, Convert.ToBoolean);
            return result;
        }

        public List<List<byte>> GetRawMatrixValues(SymbolicExpressionContext context)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            var columnCount = GetColumnCount(context);
            var rowCount = GetRowCount(context);
            var length = columnCount * rowCount;
            var values = new byte[length];
            Marshal.Copy(dataPointer, values, 0, length);

            var result = UnflattenToMatrix(values, rowCount, columnCount);
            return result;
        }

        public List<List<Complex>> GetComplexMatrixValues(SymbolicExpressionContext context)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            var columnCount = GetColumnCount(context);
            var rowCount = GetRowCount(context);
            var length = 2 * (columnCount * rowCount);
            var values = new double[length];
            Marshal.Copy(dataPointer, values, 0, length);

            var result = UnflattenToComplexMatrix(values, rowCount, columnCount);
            return result;
        }

        public string GetCharacterMatrixValueAt(SymbolicExpressionContext context, int row, int column)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            int dataSize = Marshal.SizeOf(typeof(IntPtr));
            var rowCount = GetRowCount(context);
            var offset = dataSize*(column*rowCount + row);
            var itemPointer = Marshal.ReadIntPtr(dataPointer, offset);
            if (itemPointer == GetNAString()) return null;

            var itemDataPointer = IntPtr.Add(itemPointer, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            var result = Marshal.PtrToStringAnsi(itemDataPointer);
            return result;
        }

        public void SetCharacterMatrixValueAt(SymbolicExpressionContext context, int row, int column, string value)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            int dataSize = Marshal.SizeOf(typeof(IntPtr));
            var rowCount = GetRowCount(context);
            var offset = dataSize*(column*rowCount + row);

            IntPtr charPtr = (value == null) ? GetNAString() : MakeChar(value).Handle;
            Marshal.WriteIntPtr(dataPointer, offset, charPtr);
        }

        public int GetIntegerMatrixValueAt(SymbolicExpressionContext context, int row, int column)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            var rowCount = GetRowCount(context);
            var offset = sizeof(int)*(column*rowCount + row);

            var result = Marshal.ReadInt32(dataPointer, offset);
            return result;
        }

        public void SetIntegerMatrixValueAt(SymbolicExpressionContext context, int row, int column, int value)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            var rowCount = GetRowCount(context);
            var offset = sizeof(int)*(column*rowCount + row);

            Marshal.WriteInt32(dataPointer, offset, value);
        }

        public double GetNumericMatrixValueAt(SymbolicExpressionContext context, int row, int column)
        {
            var data = new double[1];
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            var rowCount = GetRowCount(context);
            var offset = sizeof(double)*(column*rowCount + row);
            dataPointer = IntPtr.Add(dataPointer, offset);

            Marshal.Copy(dataPointer, data, 0, data.Length);
            return data[0];
        }

        public void SetNumericMatrixValueAt(SymbolicExpressionContext context, int row, int column, double value)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            var rowCount = GetRowCount(context);
            var offset = sizeof(double)*(column*rowCount + row);
            dataPointer = IntPtr.Add(dataPointer, offset);

            Marshal.Copy(new[] {value}, 0, dataPointer, 1);
        }

        public bool GetLogicalMatrixValueAt(SymbolicExpressionContext context, int row, int column)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            var rowCount = GetRowCount(context);
            var offset = sizeof(int)*(column*rowCount + row);
            var value = Marshal.ReadInt32(dataPointer, offset);

            var result = Convert.ToBoolean(value);
            return result;
        }

        public void SetLogicalMatrixValueAt(SymbolicExpressionContext context, int row, int column, bool value)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            var rowCount = GetRowCount(context);
            var offset = sizeof(int)*(column*rowCount + row);
            var coerced = Convert.ToInt32(value);

            Marshal.WriteInt32(dataPointer, offset, coerced);
        }

        public byte GetRawMatrixValueAt(SymbolicExpressionContext context, int row, int column)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            var rowCount = GetRowCount(context);
            var offset = sizeof(byte)*(column*rowCount + row);

            var result = Marshal.ReadByte(dataPointer, offset);
            return result;
        }

        public void SetRawMatrixValueAt(SymbolicExpressionContext context, int row, int column, byte value)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            var rowCount = GetRowCount(context);
            var offset = sizeof(byte)*(column*rowCount + row);

            Marshal.WriteByte(dataPointer, offset, value);
        }

        public Complex GetComplexMatrixValueAt(SymbolicExpressionContext context, int row, int column)
        {
            var data = new double[2];
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            var rowCount = GetRowCount(context);
            var offset = sizeof(double)*(column*rowCount + row)*2;
            var itemPointer = IntPtr.Add(dataPointer, offset);
            Marshal.Copy(itemPointer, data, 0, data.Length);

            var result = new Complex(data[0], data[1]);
            return result;
        }

        public void SetComplexMatrixValueAt(SymbolicExpressionContext context, int row, int column, Complex value)
        {
            var data = new[] {value.Real, value.Imaginary};
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            var rowCount = GetRowCount(context);
            var offset = sizeof(double)*(column*rowCount + row)*2;
            var itemPointer = IntPtr.Add(dataPointer, offset);

            Marshal.Copy(data, 0, itemPointer, data.Length);
        }

        public void InitializeCharacterMatrix(SymbolicExpressionContext context, List<List<string>> matrix)
        {
            var flattened = FlattenToArray(matrix);
            InitializeCharacterVector(context, flattened.ToList());
        }

        public void InitializeIntegerMatrix(SymbolicExpressionContext context, List<List<int>> matrix)
        {
            var flattened = FlattenToArray(matrix);
            InitializeIntegerVector(context, flattened.ToList());
        }

        public void InitializeNumericMatrix(SymbolicExpressionContext context, List<List<double>> matrix)
        {
            var flattened = FlattenToArray(matrix);
            InitializeNumericVector(context, flattened.ToList());
        }

        public void InitializeLogicalMatrix(SymbolicExpressionContext context, List<List<bool>> matrix)
        {
            var flattened = FlattenToArray(matrix);
            InitializeLogicalVector(context, flattened.ToList());
        }

        public void InitializeRawMatrix(SymbolicExpressionContext context, List<List<byte>> matrix)
        {
            var flattened = FlattenToArray(matrix);
            InitializeRawVector(context, flattened.ToList());
        }

        public void InitializeComplexMatrix(SymbolicExpressionContext context, List<List<Complex>> matrix)
        {
            var flattened = FlattenToArray(matrix);
            InitializeComplexVector(context, flattened.ToList());
        }

        private static double[] ConvertComplexToDouble(IList<Complex> vector)
        {
            var coerced = new double[vector.Count*2];
            for (var index = 0; index < vector.Count; index++)
            {
                var item = vector[index];
                coerced[2*index] = item.Real;
                coerced[2*index + 1] = item.Imaginary;
            }

            return coerced;
        }

        public int GetRowCount(SymbolicExpressionContext context)
        {
            var result = _instance.GetFunction<Rf_nrows>()(context.Handle);
            return result;
        }

        public int GetColumnCount(SymbolicExpressionContext context)
        {
            var result = _instance.GetFunction<Rf_ncols>()(context.Handle);
            return result;
        }

        public SymbolicExpressionContext GetParentEnvironment(SymbolicExpressionContext context)
        {
            var result = context.RawSEXP.envsxp.enclos;
            return new SymbolicExpressionContext(result);
        }

        public SymbolicExpressionContext GetValue(SymbolicExpressionContext context)
        {
            return new SymbolicExpressionContext(context.RawSEXP.symsxp.value);
        }

        public SymbolicExpressionContext GetInternal(SymbolicExpressionContext context)
        {
            return new SymbolicExpressionContext(context.RawSEXP.symsxp.@internal);
        }

        public void SetPrintName(SymbolicExpressionContext context, string value)
        {
            var strPtr = IntPtr.Add(context.RawSEXP.symsxp.pname, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            int offset = Marshal.OffsetOf(typeof(SEXPREC), "u").ToInt32() +
                         Marshal.OffsetOf(typeof(symsxp), "pname").ToInt32();
            Marshal.WriteIntPtr(context.Handle, offset, strPtr);
        }

        private static T[] FlattenToArray<T>(IReadOnlyList<IReadOnlyList<T>> matrix)
        {
            return FlattenToArray(matrix, v => v);
        }

        private static T2[] FlattenToArray<T1, T2>(IReadOnlyList<IReadOnlyList<T1>> matrix,
                                                   Func<T1, T2> converter)
        {
            var rows = matrix.Count;
            var columns = matrix.Max(s => s.Count);
            var result = new List<T2>(rows*columns);
            var length = matrix[0].Count;
            foreach (var row in matrix)
            {
                if (row.Count != length) throw new InvalidOperationException("Matrix cannot be jagged.");
                result.AddRange(row.Select(converter));
            }

            return result.ToArray();
        }

        private static List<List<T>> UnflattenToMatrix<T>(IReadOnlyList<T> values, int rowCount, int columnCount)
        {
            return UnflattenToMatrix(values, rowCount, columnCount, v => v);
        }

        private static List<List<T2>> UnflattenToMatrix<T1, T2>(IReadOnlyList<T1> values,
                                                                int rowCount,
                                                                int columnCount,
                                                                Func<T1, T2> converter)
        {
            var result = new List<List<T2>>(columnCount);
            for (var column = 0; column < columnCount; column++)
            {
                var cl = new List<T2>();
                for (var row = 0; row < rowCount; row++)
                {
                    var value = values[rowCount*column + column];
                    var converted = converter(value);
                    cl.Add(converted);
                }
                result.Add(cl);
            }
            return result;
        }

        private static List<List<Complex>> UnflattenToComplexMatrix(IReadOnlyList<double> values,
                                                                int rowCount,
                                                                int columnCount)
        {
            var result = new List<List<Complex>>(columnCount);
            for (var column = 0; column < columnCount; column++)
            {
                var cl = new List<Complex>();
                for (var row = 0; row < rowCount * 2; row += 2)
                {
                    var index = rowCount*column*2 + row;
                    cl.Add(new Complex(values[index], values[index + 1]));
                }
                result.Add(cl);
            }
            return result;
        }

        public SymbolicExpressionContext GetBaseNamespace()
        {
            var symbol = GetPredefinedSymbol("R_BaseNamespace");
            return new SymbolicExpressionContext(symbol);
        }

        public SymbolicExpressionContext GetClass()
        {
            var symbol = GetPredefinedSymbol("R_ClassSymbol");
            return new SymbolicExpressionContext(symbol);
        }

        public SymbolicExpressionContext GetEmptyEnvironment()
        {
            var symbol = GetPredefinedSymbol("R_EmptyEnv");
            return new SymbolicExpressionContext(symbol);
        }

        public List<int> GetIntegerValues(SymbolicExpressionContext context)
        {
            var length = GetLength(context);
            var values = new int[length];
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            Marshal.Copy(values, 0, dataPointer, values.Length);

            return values.ToList();
        }

        public string GetInternalCharacterValue(SymbolicExpressionContext context)
        {
            var dataPointer = IntPtr.Add(context.Handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            var result = Marshal.PtrToStringAnsi(dataPointer);
            return result;
        }

        public SymbolicExpressionContext GetLevels()
        {
            var symbol = GetPredefinedSymbol("R_LevelsSymbol");
            return new SymbolicExpressionContext(symbol);
        }

        public IntPtr GetNAString()
        {
            var symbol = GetPredefinedSymbol("R_NaString");
            return symbol;
        }

        private IntPtr GetPredefinedSymbol(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            var pointer = _instance.GetPointer(name);
            return Marshal.ReadIntPtr(pointer);
        }

        public SymbolicExpressionContext GetUnbound()
        {
            var result = GetPredefinedSymbol("R_UnboundValue");
            return new SymbolicExpressionContext(result);
        }

        public bool IsString(SymbolicExpressionContext allocated)
        {
            var result = _instance.GetFunction<Rf_isString>()(allocated.Handle);
            return result;
        }

        public bool IsNull(SymbolicExpressionContext allocated)
        {
            var result = _instance.GetFunction<Rf_isNull>()(allocated.Handle);
            return result;
        }

        public bool IsPairList(SymbolicExpressionContext allocated)
        {
            var result = _instance.GetFunction<Rf_isPairList>()(allocated.Handle);
            return result;
        }
    }
}
