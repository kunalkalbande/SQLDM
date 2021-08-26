using System;
//using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Microsoft.Data.Schema.ScriptDom.Sql;
using System.Reflection;
using System.Diagnostics;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Helpers
{
    public class TSqlNode
    {
        private TSqlFragment parent;
        private TSqlFragment child;

        public TSqlNode(TSqlFragment parent, TSqlFragment child)
        {
            this.parent = parent;
            this.child = child;
        }

        public TSqlFragment Child
        {
            get { return child; }
            set { child = value; }
        }

        public TSqlFragment Parent
        {
            get { return parent; }
            set { parent = value; }
        }
    }

    public static class TSqlParsingHelpers
    {
        public static TSqlParser GetParser(ServerVersion version, bool initialQuotedIdentifiers)
        {
            switch (version.Major)
            {
                case 8:
                    return new TSql80Parser(initialQuotedIdentifiers);
                case 9:
                    return new TSql90Parser(initialQuotedIdentifiers);
                default:
                    return new TSql100Parser(initialQuotedIdentifiers);
            }
        }

        public static IList<TSqlFragment> GetChildren(TSqlScript script)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>(script.Batches.Count);
            foreach (TSqlFragment fragment in script.Batches)
                result.Add(fragment);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(TSqlBatch batch)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>(batch.Statements.Count);
            foreach (TSqlFragment fragment in batch.Statements)
                result.Add(fragment);
            return result;
        }

        #region statements

        public static IList<TSqlFragment> GetChildren(InsertStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.InsertSource != null)
                result.Add(stmt.InsertSource);
            foreach (TSqlFragment frag in stmt.Columns)
                result.Add(frag);
            if (stmt.Target != null)
                result.Add(stmt.Target);
            if (stmt.TopRowFilter != null)
                result.Add(stmt.TopRowFilter);
            if (stmt.OutputClause != null)
                result.Add(stmt.OutputClause);
            foreach (TSqlFragment frag in stmt.OptimizerHints)
                result.Add(frag);
            if (stmt.WithCommonTableExpressionsAndXmlNamespaces != null)
                result.Add(stmt.WithCommonTableExpressionsAndXmlNamespaces);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(SelectStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.QueryExpression != null)
                result.Add(stmt.QueryExpression);
            if (stmt.OrderByClause != null)
                result.Add(stmt.OrderByClause);
            foreach (TSqlFragment frag in stmt.ComputeClauses)
                result.Add(frag);
            if (stmt.ForClause != null)
                result.Add(stmt.ForClause);
            foreach (TSqlFragment frag in stmt.OptimizerHints)
                result.Add(frag);
            if (stmt.WithCommonTableExpressionsAndXmlNamespaces != null)
                result.Add(stmt.WithCommonTableExpressionsAndXmlNamespaces);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(DeleteStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();

            if (stmt.TopRowFilter != null)
                result.Add(stmt.TopRowFilter);

            foreach (TSqlFragment frag in stmt.FromClauses)
                result.Add(frag);

            if (stmt.OutputClause != null)
                result.Add(stmt.OutputClause);

            if (stmt.WhereClause != null)
                result.Add(stmt.WhereClause);

            foreach (TSqlFragment frag in stmt.OptimizerHints)
                result.Add(frag);

            if (stmt.WithCommonTableExpressionsAndXmlNamespaces != null)
                result.Add(stmt.WithCommonTableExpressionsAndXmlNamespaces);

            return result;
        }

        public static IList<TSqlFragment> GetChildren(ExecuteStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();

            if (stmt.Variable != null)
                result.Add(stmt.Variable);
            if (stmt.LinkedServer != null)
                result.Add(stmt.LinkedServer);
            if (stmt.ExecuteContext != null)
                result.Add(stmt.ExecuteContext);
            if (stmt.ExecutableEntity != null)
                result.Add(stmt.ExecutableEntity);
            if (stmt.LinkedServer != null)
                result.Add(stmt.LinkedServer);

            return result;
        }

        public static IList<TSqlFragment> GetChildren(AlterProcedureStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.ExecuteAs != null)
                result.Add(stmt.ExecuteAs);
            if (stmt.MethodSpecifier != null)
                result.Add(stmt.MethodSpecifier);
            foreach (TSqlFragment frag in stmt.Parameters)
                result.Add(frag);
            if (stmt.ProcedureReference != null)
                result.Add(stmt.ProcedureReference);
            if (stmt.StatementList != null)
                result.Add(stmt.StatementList);

            return result;
        }

        public static IList<TSqlFragment> GetChildren(AlterIndexStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.Partition != null)
                result.Add(stmt.Partition);
            if (stmt.Name != null)
                result.Add(stmt.Name);
            if (stmt.OnName != null)
                result.Add(stmt.OnName);
            foreach (TSqlFragment frag in stmt.IndexOptions)
                result.Add(frag);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(ReconfigureStatement stmt)
        {
            return new List<TSqlFragment>();
        }

        public static IList<TSqlFragment> GetChildren(CreateTableStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.SchemaObjectName != null)
                result.Add(stmt.SchemaObjectName);
            if (stmt.OnFileGroupOrPartitionScheme != null)
                result.Add(stmt.OnFileGroupOrPartitionScheme);
            if (stmt.TextImageOn != null)
                result.Add(stmt.TextImageOn);
            foreach (TSqlFragment frag in stmt.DataCompressionOptions)
                result.Add(frag);
            foreach (TSqlFragment frag in stmt.ColumnDefinitions)
                result.Add(frag);
            foreach (TSqlFragment frag in stmt.TableConstraints)
                result.Add(frag);
            if (stmt.FileStreamOn != null)
                result.Add(stmt.FileStreamOn);

            return result;
        }

        public static IList<TSqlFragment> GetChildren(CursorDefinition stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.Select != null)
                result.Add(stmt.Select);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(CreateProcedureStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.ExecuteAs != null)
                result.Add(stmt.ExecuteAs);
            if (stmt.MethodSpecifier != null)
                result.Add(stmt.MethodSpecifier);
            foreach (TSqlFragment frag in stmt.Parameters)
                result.Add(frag);
            if (stmt.ProcedureReference != null)
                result.Add(stmt.ProcedureReference);
            if (stmt.StatementList != null)
                result.Add(stmt.StatementList);

            return result;
        }

        public static IList<TSqlFragment> GetChildren(IfStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.Predicate != null)
                result.Add(stmt.Predicate);
            if (stmt.ThenStatement != null)
                result.Add(stmt.ThenStatement);
            if (stmt.ElseStatement != null)
                result.Add(stmt.ElseStatement);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(LabelStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            return result;
        }

        public static IList<TSqlFragment> GetChildren(GoToStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.LabelName != null)
                result.Add(stmt.LabelName);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(ReturnStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.Expression != null)
                result.Add(stmt.Expression);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(BeginTransactionStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.MarkDescription != null)
                result.Add(stmt.MarkDescription);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(CommitTransactionStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.Name != null)
                result.Add(stmt.Name);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(SetCommandStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            foreach (TSqlFragment frag in stmt.Commands)
                result.Add(frag);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(SetTextSizeStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();

            if (stmt.TextSize != null)
                result.Add(stmt.TextSize);

            return result;
        }

        public static IList<TSqlFragment> GetChildren(RaiseErrorStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();

            if (stmt.FirstParameter != null)
                result.Add(stmt.FirstParameter);
            if (stmt.SecondParameter != null)
                result.Add(stmt.SecondParameter);
            if (stmt.ThirdParameter != null)
                result.Add(stmt.ThirdParameter);
            if (stmt.OptionalParameters != null)
            {
                foreach (Expression e in stmt.OptionalParameters)
                    result.Add(e);
            }
            return result;
        }

        public static IList<TSqlFragment> GetChildren(CloseCursorStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            return result;
        }

        public static IList<TSqlFragment> GetChildren(WhileStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();

            if (stmt.Predicate != null)
                result.Add(stmt.Predicate);
            if (stmt.Statement != null)
                result.Add(stmt.Statement);

            return result;
        }

        public static IList<TSqlFragment> GetChildren(FetchCursorStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();

            if (stmt.FetchType != null)
                result.Add(stmt.FetchType);
            foreach (TSqlFragment frag in stmt.IntoVariables)
                result.Add(frag);

            return result;
        }

        public static IList<TSqlFragment> GetChildren(DeallocateCursorStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            return result;
        }

        public static IList<TSqlFragment> GetChildren(OpenCursorStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            return result;
        }

        public static IList<TSqlFragment> GetChildren(DeclareCursorStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();

            if (stmt.Name != null)
                result.Add(stmt.Name);

            if (stmt.CursorDefinition != null)
                result.Add(stmt.CursorDefinition);

            return result;
        }

        public static IList<TSqlFragment> GetChildren(TruncateTableStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.TableName != null)
                result.Add(stmt.TableName);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(WaitForStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.Parameter != null)
                result.Add(stmt.Parameter);
            if (stmt.Timeout != null)
                result.Add(stmt.Timeout);
            if (stmt.Statement != null)
                result.Add(stmt.Statement);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(UpdateStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();

            foreach (TSqlFragment frag in stmt.SetClauses)
                result.Add(frag);

            foreach (TSqlFragment frag in stmt.FromClauses)
                result.Add(frag);

            if (stmt.WhereClause != null)
                result.Add(stmt.WhereClause);

            if (stmt.Target != null)
                result.Add(stmt.Target);

            if (stmt.TopRowFilter != null)
                result.Add(stmt.TopRowFilter);

            if (stmt.OutputClause != null)
                result.Add(stmt.OutputClause);

            foreach (TSqlFragment frag in stmt.OptimizerHints)
                result.Add(frag);

            if (stmt.WithCommonTableExpressionsAndXmlNamespaces != null)
                result.Add(stmt.WithCommonTableExpressionsAndXmlNamespaces);

            return result;
        }

        public static IList<TSqlFragment> GetChildren(DropTableStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            return result;
        }

        public static IList<TSqlFragment> GetChildren(DbccStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            return result;
        }

        public static IList<TSqlFragment> GetChildren(StatementList stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            foreach (TSqlFragment frag in stmt.Statements)
                result.Add(frag);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(UseStatement stmt)
        {
            return new List<TSqlFragment>();
        }

        public static IList<TSqlFragment> GetChildren(PredicateSetStatement stmt)
        {
            return new List<TSqlFragment>();
        }

        public static IList<TSqlFragment> GetChildren(BeginEndBlockStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.StatementList != null)
                result.Add(stmt.StatementList);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(ProcedureParameter stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.DataType != null)
                result.Add(stmt.DataType);
            if (stmt.Default != null)
                result.Add(stmt.Default);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(ProcedureReference stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.Name != null)
                result.Add(stmt.Name);
            if (stmt.Number != null)
                result.Add(stmt.Number);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(DeclareVariableStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            foreach (TSqlFragment frag in stmt.Declarations)
                result.Add(frag);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(DeclareVariableElement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.DataType != null)
                result.Add(stmt.DataType);
            if (stmt.InitialValue != null)
                result.Add(stmt.InitialValue);
            if (stmt.VariableName != null)
                result.Add(stmt.VariableName);

            return result;
        }

        public static IList<TSqlFragment> GetChildren(DeclareTableStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.Body != null)
                result.Add(stmt.Body);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(GroupByClause stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            foreach (TSqlFragment frag in stmt.GroupingSpecifications)
                result.Add(frag);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(GroupingSpecification stmt)
        {
            return new List<TSqlFragment>();
        }

        public static IList<TSqlFragment> GetChildren(WhenClause stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.WhenExpression != null)
                result.Add(stmt.WhenExpression);
            if (stmt.ThenExpression != null)
                result.Add(stmt.ThenExpression);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(VariableDataModificationTarget stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.Variable != null)
                result.Add(stmt.Variable);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(DeclareTableBody stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.VariableName != null)
                result.Add(stmt.VariableName);
            foreach (TSqlFragment frag in stmt.ColumnDefinitions)
                result.Add(frag);
            foreach (TSqlFragment frag in stmt.TableConstraints)
                result.Add(frag);

            return result;
        }

        public static IList<TSqlFragment> GetChildren(SetVariableStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.VariableName != null)
                result.Add(stmt.VariableName);
            if (stmt.Identifier != null)
                result.Add(stmt.Identifier);
            foreach (TSqlFragment frag in stmt.Parameters)
                result.Add(frag);
            if (stmt.Expression != null)
                result.Add(stmt.Expression);
            if (stmt.CursorDefinition != null)
                result.Add(stmt.CursorDefinition);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(TryCatchStatement stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.TryStatements != null)
                result.Add(stmt.TryStatements);
            if (stmt.CatchStatements != null)
                result.Add(stmt.CatchStatements);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(XmlForClause stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            foreach (TSqlFragment frag in stmt.Options)
                result.Add(frag);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(OrderByClause stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            foreach (TSqlFragment frag in stmt.OrderByElements)
                result.Add(frag);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(ReadOnlyForClause stmt)
        {
            return new List<TSqlFragment>();
        }

        public static IList<TSqlFragment> GetChildren(SqlDataType stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.Name != null)
                result.Add(stmt.Name);
            foreach (TSqlFragment frag in stmt.Parameters)
                result.Add(frag);
            return result;
        }
        public static IList<TSqlFragment> GetChildren(ExpressionWithSortOrder stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.Expression != null)
                result.Add(stmt.Expression);
            return result;
        }
        public static IList<TSqlFragment> GetChildren(NullableConstraint stmt)
        {
            return new List<TSqlFragment>();
        }
        public static IList<TSqlFragment> GetChildren(SimpleTableHint stmt)
        {
            return new List<TSqlFragment>();
        }
        public static IList<TSqlFragment> GetChildren(XmlForClauseOption stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.Value != null)
                result.Add(stmt.Value);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(VariableTableSource stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.Name != null)
                result.Add(stmt.Name);
            if (stmt.FunctionCall != null)
                result.Add(stmt.FunctionCall);
            foreach (TSqlFragment frag in stmt.Columns)
                result.Add(frag);
            if (stmt.Alias != null)
                result.Add(stmt.Alias);
            return result;
        }
        public static IList<TSqlFragment> GetChildren(QualifiedJoin stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.FirstTableSource != null)
                result.Add(stmt.FirstTableSource);
            if (stmt.SecondTableSource != null)
                result.Add(stmt.SecondTableSource);
            if (stmt.SearchCondition != null)
                result.Add(stmt.SearchCondition);
            return result;
        }
        public static IList<TSqlFragment> GetChildren(WhereClause stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.SearchCondition != null)
                result.Add(stmt.SearchCondition);
            if (stmt.Cursor != null)
                result.Add(stmt.Cursor);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(ColumnDefinition stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (stmt.ComputedColumnExpression != null)
                result.Add(stmt.ComputedColumnExpression);
            if (stmt.DataType != null)
                result.Add(stmt.DataType);
            if (stmt.DefaultConstraint != null)
                result.Add(stmt.DefaultConstraint);
            if (stmt.IsIdentity)
            {
                if (stmt.IdentityIncrement != null)
                    result.Add(stmt.IdentityIncrement);
                if (stmt.IdentitySeed != null)
                    result.Add(stmt.IdentitySeed);
            }
            if (stmt.StorageOptions != null)
                result.Add(stmt.StorageOptions);

            foreach (TSqlFragment frag in stmt.Constraints)
                result.Add(frag);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(Identifier stmt)
        {
            return new List<TSqlFragment>();
        }

        public static IList<TSqlFragment> GetChildren(IdentifiersCallTarget stmt)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            foreach (Identifier id in stmt.Identifiers)
                result.Add(id);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(SetTransactionIsolationLevelStatement stmt)
        {
            return new List<TSqlFragment>();
        }

        #endregion

        public static IList<TSqlFragment> GetChildren(AssignmentSetClause exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            if (exp.Variable != null)
                result.Add(exp.Variable);
            if (exp.NewValue != null)
                result.Add(exp.NewValue);
            if (exp.Column != null)
                result.Add(exp.Column);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(ExecutableStringList exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            foreach (Literal lit in exp.Strings)
                result.Add(lit);
            foreach (ExecuteParameter parm in exp.Parameters)
                result.Add(parm);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(FetchType exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            if (exp.RowOffset != null)
                result.Add(exp.RowOffset);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(SelectColumn exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            if (exp.ColumnName != null)
                result.Add(exp.ColumnName);
            if (exp.Expression != null)
                result.Add(exp.Expression);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(SchemaObjectName exp)
        {
            return new List<TSqlFragment>(1);
        }

        public static IList<TSqlFragment> GetChildren(SelectSetVariable exp)
        {

            IList<TSqlFragment> result = new List<TSqlFragment>();
            if (exp.VariableName != null)
                result.Add(exp.VariableName);

            if (exp.Expression != null)
                result.Add(exp.Expression);

            return result;
        }

        public static IList<TSqlFragment> GetChildren(UserDataType exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            if (exp.Parameters != null)
            {
                foreach (Literal lit in exp.Parameters)
                    result.Add(lit);
            }

            if (exp.Name != null)
                result.Add(exp.Name);

            return result;
        }

        public static IList<TSqlFragment> GetChildren(DefaultConstraint exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();

            if (exp.Expression != null)
                result.Add(exp.Expression);
            if (exp.Column != null)
                result.Add(exp.Column);
            if (exp.ConstraintIdentifier != null)
                result.Add(exp.ConstraintIdentifier);

            return result;
        }

        public static IList<TSqlFragment> GetChildren(GeneralSetCommand exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            if (exp.Parameter != null)
                result.Add(exp.Parameter);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(ExecutableProcedureReference exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            if (exp.ProcedureReference != null)
                result.Add(exp.ProcedureReference);

            if (exp.AdhocDataSource != null)
                result.Add(exp.AdhocDataSource);

            if (exp.Parameters != null)
            {
                foreach (ExecuteParameter parm in exp.Parameters)
                {
                    result.Add(parm);
                }
            }
            return result;
        }

        public static IList<TSqlFragment> GetChildren(ExecuteParameter exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            if (exp.Variable != null)
                result.Add(exp.Variable);
            if (exp.ParameterValue != null)
                result.Add(exp.ParameterValue);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(WithCommonTableExpressionsAndXmlNamespaces exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();

            foreach (var cte in exp.CommonTableExpressions)
                result.Add(cte);
            result.Add(exp.XmlNamespaces);

            return result;
        }

        public static IList<TSqlFragment> GetChildren(CommonTableExpression exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            result.Add(exp.Subquery);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(XmlNamespaces exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            foreach (var cte in exp.XmlNamespacesElements)
                result.Add(cte);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(XmlNamespacesElement exp)
        {
            return new List<TSqlFragment>(1);
        }

        public static IList<TSqlFragment> GetChildren(XmlNamespacesDefaultElement exp)
        {
            return new List<TSqlFragment>(1);
        }

        public static IList<TSqlFragment> GetChildren(XmlNamespacesAliasElement exp)
        {
            return new List<TSqlFragment>(1);
        }

        public static IList<TSqlFragment> GetChildren(BreakStatement exp)
        {
            return new List<TSqlFragment>(1);
        }

        public static IList<TSqlFragment> GetChildren(InsertBulkStatement exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            List<TSqlFragment> temp;
            if (exp.ColumnDefinitions != null)
            {
                temp = new List<TSqlFragment>();
                foreach (InsertBulkColumnDefinition item in exp.ColumnDefinitions)
                {
                    temp.Add((TSqlFragment)item);
                }
                //result.AddRange(exp.ColumnDefinitions.Cast<TSqlFragment>());
                result.AddRange(temp);
            }

            if (exp.Options != null)
            {
                temp = new List<TSqlFragment>();
                foreach (BulkInsertOption item in exp.Options)
                {
                    temp.Add((TSqlFragment)item);
                }
                //result.AddRange(exp.Options.Cast<TSqlFragment>());
                result.AddRange(temp);
            }

            if (exp.To != null)
                result.Add(exp.To);

            return result;
        }

        public static IList<TSqlFragment> GetChildren(InsertBulkColumnDefinition exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            result.Add(exp.Column);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(InternalOpenRowset exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            if (exp.VarArgs != null)
            {
                foreach (Expression x in exp.VarArgs)
                    result.Add(x);
            }
            return result;
        }


        #region TableSources

        public static IList<TSqlFragment> GetChildren(SchemaObjectTableSource exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            if (exp.SchemaObject != null)
                result.Add(exp.SchemaObject);
            if (exp.ParametersUsed)
                result.AddRange(GetChildren(exp.Parameters));
            foreach (TSqlFragment frag in exp.TableHints)
                result.Add(frag);
            if (exp.TableSampleClause != null)
                result.Add(exp.TableSampleClause);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(BuiltInFunctionTableSource exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            if (exp.Name != null)
                result.Add(exp.Name);
            if (exp.Parameters != null)
                result.AddRange(GetChildren(exp.Parameters));
            if (exp.Alias != null)
                result.Add(exp.Alias);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(QueryDerivedTable exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            foreach (TSqlFragment frag in exp.Columns)
                result.Add(frag);
            if (exp.Subquery != null)
                result.Add(exp.Subquery);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(TopRowFilter exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            if (exp.Expression != null)
                result.Add(exp.Expression);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(HavingClause exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();

            if (exp.SearchCondition != null)
                result.Add(exp.SearchCondition);

            return result;
        }

        public static IList<TSqlFragment> GetChildren(SchemaObjectDataModificationTarget exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();

            if (exp.SchemaObject != null)
                result.Add(exp.SchemaObject);
            if (exp.Parameters != null)
                result.AddRange(GetChildren(exp.Parameters));
            if (exp.TableHints != null)
                result.AddRange(GetChildren(exp.TableHints));

            return result;
        }

        public static IList<TSqlFragment> GetChildren(SimpleOptimizerHint exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();

            if (exp.Value != null)
                result.Add(exp.Value);

            return result;
        }

        public static IList<TSqlFragment> GetChildren(ValuesInsertSource exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            foreach (RowValue value in exp.RowValues)
                result.Add(value);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(RowValue exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();

            if (exp.ColumnValues != null)
                result.AddRange(GetChildren(exp.ColumnValues));

            return result;
        }

        #endregion

        #region Expression

        public static IList<TSqlFragment> GetChildren(InPredicate exp)
        {
            IList<TSqlFragment> result = GetChildren(exp.Values);
            result.Add(exp.Expression);
            result.Add(exp.Subquery);

            return result;
        }

        public static IList<TSqlFragment> GetChildren(BinaryExpression exp)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            result.Add(exp.FirstExpression);
            result.Add(exp.SecondExpression);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(TernaryExpression exp)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            result.Add(exp.FirstExpression);
            result.Add(exp.SecondExpression);
            result.Add(exp.ThirdExpression);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(FullTextPredicate exp)
        {
            IList<TSqlFragment> result = GetChildren(exp.Columns);
            result.Add(exp.LanguageTerm);
            result.Add(exp.Value);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(ExtractFromExpression exp)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            result.Add(exp.Expression);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(OdbcConvertSpecification exp)
        {
            return new List<TSqlFragment>();
        }
        public static IList<TSqlFragment> GetChildren(UpdateCall exp)
        {
            return new List<TSqlFragment>();
        }
        public static IList<TSqlFragment> GetChildren(UnaryExpression exp)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            result.Add(exp.Expression);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(SourceDeclaration exp)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            result.Add(exp.Value);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(IdentityFunction exp)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            result.Add(exp.Increment);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(ExistsPredicate exp)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            result.Add(exp.Subquery);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(EventDeclarationCompareFunctionParameter exp)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            result.Add(exp.SourceDeclaration);
            result.Add(exp.EventValue);

            return result;
        }
        public static IList<TSqlFragment> GetChildren(LikePredicate exp)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            result.Add(exp.EscapeExpression);
            result.Add(exp.FirstExpression);
            result.Add(exp.SecondExpression);

            return result;
        }
        public static IList<TSqlFragment> GetChildren(SubqueryComparisonPredicate exp)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            result.Add(exp.Subquery);
            result.Add(exp.Expression);
            return result;
        }
        public static IList<TSqlFragment> GetChildren(TSEqualCall exp)
        {
            IList<TSqlFragment> result = new List<TSqlFragment>();
            result.Add(exp.FirstExpression);
            result.Add(exp.SecondExpression);
            return result;
        }


        #endregion

        #region QueryExpression

        public static IList<TSqlFragment> GetChildren(BinaryQueryExpression exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            result.Add(exp.FirstQueryExpression);
            result.Add(exp.SecondQueryExpression);
            return result;
        }
        public static IList<TSqlFragment> GetChildren(QuerySpecification exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            if (exp.TopRowFilter != null)
                result.Add(exp.TopRowFilter);
            if (exp.Into != null)
                result.Add(exp.Into);
            if (exp.SelectElements != null && exp.SelectElements.Count > 0)
                result.AddRange(exp.SelectElements);
            if (exp.FromClauses != null && exp.FromClauses.Count > 0)
                result.AddRange(GetChildren(exp.FromClauses));
            if (exp.WhereClause != null)
                result.Add(exp.WhereClause);
            if (exp.GroupByClause != null)
                result.Add(exp.GroupByClause);
            if (exp.HavingClause != null)
                result.Add(exp.HavingClause);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(QueryParenthesis exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            result.Add(exp.QueryExpression);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(SubquerySpecification exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            result.Add(exp.TopRowFilter);
            result.Add(exp.Into);
            result.AddRange(exp.SelectElements);
            result.AddRange(GetChildren(exp.FromClauses));
            result.Add(exp.WhereClause);
            result.Add(exp.GroupByClause);
            result.Add(exp.HavingClause);
            result.Add(exp.OrderByClause);
            result.Add(exp.XmlForClause);

            return result;
        }

        #endregion

        #region PrimaryExpression

        public static IList<TSqlFragment> GetChildren(Column column)
        {
            return new List<TSqlFragment>();
        }

        public static IList<TSqlFragment> GetChildren(Literal literal)
        {
            return new List<TSqlFragment>();
        }

        public static IList<TSqlFragment> GetChildren(LeftFunctionCall lfc)
        {
            return GetChildren(lfc.Parameters);
        }

        public static IList<TSqlFragment> GetChildren(CoalesceExpression exp)
        {
            return GetChildren(exp.Expressions);
        }

        public static IList<TSqlFragment> GetChildren(ParenthesisExpression exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            result.Add(exp.Expression);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(PartitionFunctionCall exp)
        {
            return GetChildren(exp.Parameters);
        }

        public static IList<TSqlFragment> GetChildren(ConvertCall exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            result.Add(exp.Parameter);
            result.Add(exp.Style);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(NullIfExpression exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            result.Add(exp.FirstExpression);
            result.Add(exp.SecondExpression);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(CastCall exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            result.Add(exp.Parameter);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(CaseExpression exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            result.Add(exp.InputExpression);
            foreach (WhenClause whenClause in exp.WhenClauses)
                result.Add(whenClause);
            result.Add(exp.ElseExpression);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(FunctionCall exp)
        {
            IList<TSqlFragment> result = GetChildren(exp.Parameters);
            result.Add(exp.OverClause);
            result.Add(exp.CallTarget);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(ParameterlessCall exp)
        {
            return new List<TSqlFragment>();
        }

        public static IList<TSqlFragment> GetChildren(UserDefinedTypePropertyAccess exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            result.Add(exp.CallTarget);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(Subquery exp)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            result.Add(exp.QueryExpression);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(RightFunctionCall exp)
        {
            return GetChildren(exp.Parameters);
        }

        public static IList<TSqlFragment> GetChildren(OdbcFunctionCall exp)
        {
            return GetChildren(exp.Parameters);
        }

        #endregion

        #region helpers

        public static BufferLocation GetLocation(this TSqlFragment fragment)
        {
            return GetBufferLocation(fragment);
        }

        public static BufferLocation GetBufferLocation(TSqlFragment fragment)
        {
            return new BufferLocation(fragment.StartOffset, fragment.StartLine, fragment.StartColumn);
        }

        public static BufferLocation GetLocation(this TSqlParserToken token)
        {
            return GetBufferLocation(token);
        }

        public static BufferLocation GetBufferLocation(TSqlParserToken token)
        {
            return new BufferLocation(token.Offset, token.Line, token.Column);
        }

        public static SelectionRectangle GetSelectionBounds(this TSqlFragment fragment)
        {
            return GetSelectionRectangle(fragment);
        }

        public static SelectionRectangle GetSelectionRectangle(TSqlFragment fragment)
        {
            BufferLocation loc = GetBufferLocation(fragment);
            int tix = fragment.LastTokenIndex;
            if (tix == -1)
                tix = fragment.ScriptTokenStream.Count - 1;
            TSqlParserToken lastToken = fragment.ScriptTokenStream[tix];
            int tlen = lastToken.Text.Length;
            return new SelectionRectangle(loc, lastToken.Offset - fragment.StartOffset + tlen);
        }

        public static SelectionRectangle GetSelectionBounds(this TSqlParserToken token)
        {
            return GetSelectionRectangle(token);
        }

        public static SelectionRectangle GetSelectionRectangle(TSqlParserToken token)
        {
            BufferLocation loc = GetBufferLocation(token);
            return new SelectionRectangle(loc, token.Text.Length);
        }

        public static IList<TSqlFragment> GetChildren(TSqlFragment fragment)
        {
            Type[] ptypes = new Type[] { fragment.GetType() };

            MethodInfo method = typeof(TSqlParsingHelpers).GetMethod("GetChildren", ptypes);
            if (method == null)
                return new List<TSqlFragment>();

            // prevent recursion from missing GetChildren implementations
            MethodBase thisMethod = MethodInfo.GetCurrentMethod();
            if (thisMethod.MethodHandle == method.MethodHandle)
            {
                Debug.Print("Unhandled child type: {0}", fragment.GetType().Name);
                return new List<TSqlFragment>();
            }

            List<TSqlFragment> result = method.Invoke(null, new object[] { fragment }) as List<TSqlFragment>;

            return result ?? new List<TSqlFragment>();
        }

        public static IList<TSqlFragment> GetChildren(IList<Column> expressions)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            foreach (TSqlFragment frag in expressions)
                result.Add(frag);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(IList<Expression> expressions)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            foreach (TSqlFragment frag in expressions)
                result.Add(frag);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(IList<TableSource> expressions)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            foreach (TSqlFragment frag in expressions)
                result.Add(frag);
            return result;
        }

        public static IList<TSqlFragment> GetChildren(IList<TableHint> expressions)
        {
            List<TSqlFragment> result = new List<TSqlFragment>();
            foreach (TSqlFragment frag in expressions)
                result.Add(frag);
            return result;
        }

        public static IEnumerator<TSqlNode> GetBreadthFirstEnumerator(TSqlFragment fragment, bool returnFragment)
        {
            Queue<TSqlNode> q = new Queue<TSqlNode>();
            q.Enqueue(new TSqlNode(null, fragment));

            while (q.Count > 0)
            {
                TSqlNode node = q.Dequeue();

                TSqlFragment parent = node.Child;
                IList<TSqlFragment> children = GetChildren(parent);
                if (children != null && children.Count > 0)
                {
                    foreach (TSqlFragment child in children)
                    {
                        if (child == null)
                            continue;
                        q.Enqueue(new TSqlNode(parent, child));
                    }
                }
                if (!returnFragment)
                {
                    returnFragment = true;
                    continue;
                }
                yield return node;
            }
        }

        public static IEnumerator<TSqlFragment> GetDepthFirstEnumerator(TSqlFragment fragment, bool returnFragment)
        {
            if (returnFragment)
                yield return fragment;

            IList<TSqlFragment> children = GetChildren(fragment);
            if (children != null && children.Count > 0)
            {
                foreach (TSqlFragment child in children)
                {
                    if (child == null)
                        continue;
                    IEnumerator<TSqlFragment> childEnum = GetDepthFirstEnumerator(child, true);
                    while (childEnum.MoveNext())
                        yield return childEnum.Current;
                }
            }
        }

        public static IEnumerator<TSqlFragment> GetDepthFirstEnumerator(IList<TSqlFragment> children)
        {
            if (children != null && children.Count > 0)
            {
                foreach (TSqlFragment child in children)
                {
                    if (child == null)
                        continue;
                    IEnumerator<TSqlFragment> childEnum = GetDepthFirstEnumerator(child, true);
                    while (childEnum.MoveNext())
                        yield return childEnum.Current;
                }
            }
        }


        public static IEnumerator<TSqlFragment> GetBottomUpEnumerator(TSqlFragment fragment, bool returnFragment)
        {
            Stack<TSqlFragment> stack = new Stack<TSqlFragment>();

            //         IEnumerator<TSqlFragment> enumerator = GetBreadthFirstEnumerator(fragment, returnFragment);
            //         while (enumerator.MoveNext())
            //         {
            //             stack.Push(enumerator.Current);
            //         }
            //         while (stack.Count > 0)
            //         {
            //             yield return stack.Pop();
            //         }
            return null;
        }

        public static Dictionary<string, TableSource> GetTableSources(SelectStatement statement)
        {
            QuerySpecification spec = statement.QueryExpression as QuerySpecification;
            if (spec != null)
            {
                return GetTableSources(spec.FromClauses);
            }

            return new Dictionary<string, TableSource>();
        }

        public static Dictionary<string, TableSource> GetTableSources(IList<TableSource> tableSources)
        {
            if (tableSources != null)
            {
                Dictionary<string, TableSource> result = new Dictionary<string, TableSource>();

                Queue<TableSource> sources = new Queue<TableSource>(tableSources);
                while (sources.Count > 0)
                {
                    TableSource source = sources.Dequeue();
                    if (source is TableSourceWithAlias)
                    {
                        string tableName = null;
                        Identifier aliasId = ((TableSourceWithAlias)source).Alias;
                        if (aliasId != null)
                            tableName = aliasId.Value;
                        if (String.IsNullOrEmpty(tableName))
                        {
                            if (source is SchemaObjectTableSource)
                                tableName = ((SchemaObjectTableSource)source).SchemaObject.BaseIdentifier.Value;
                            if (source is VariableTableSource)
                                tableName = ((VariableTableSource)source).Name.Value;
                        }
                        if (!String.IsNullOrEmpty(tableName) && !result.ContainsKey(tableName))
                        {
                            result.Add(tableName.ToLower(), source);
                        }
                    }
                    else
                    {   // drill one level into a table source to get tables
                        foreach (TSqlFragment fragment in GetChildren(source))
                        {
                            if (fragment is TableSource)
                                sources.Enqueue((TableSource)fragment);
                        }
                    }
                }
                return result;
            }
            return null;
        }

        public static string TryGetTableName(string aliasOrTableName, Dictionary<string, TableSource> tableMap)
        {
            TableSource selected = null;
            if (String.IsNullOrEmpty(aliasOrTableName))
            {
                foreach (TableSource source in tableMap.Values)
                {
                    if (source is VariableTableSource || source is SchemaObjectTableSource)
                    {
                        selected = source;
                        break;
                    }
                }
            }
            else
            {
                if (!tableMap.TryGetValue(aliasOrTableName.ToLower(), out selected))
                {
                    return null;
                }
            }

            if (selected is VariableTableSource)
                return ((VariableTableSource)selected).Name.Value;
            if (selected is SchemaObjectTableSource)
                return ((SchemaObjectTableSource)selected).SchemaObject.BaseIdentifier.Value;

            return null;
        }

        public static int GetTableSourceCount(IEnumerable<TableSource> tables, bool countTempTables, bool countTableVariables)
        {
            int result = 0;
            foreach (TableSource tableSource in tables)
            {
                if (countTempTables && tableSource is SchemaObjectTableSource)
                {
                    string tableName = ((SchemaObjectTableSource)tableSource).SchemaObject.BaseIdentifier.Value;
                    if (!String.IsNullOrEmpty(tableName) && tableName[0] == '#')
                    {
                        result++;
                    }
                }
                else
                    if (countTableVariables && tableSource is VariableTableSource)
                    {
                        result++;
                    }
            }
            return result;
        }

        public static string GetNormalizedName(IList<Identifier> ids)
        {
            return (GetNormalizedName(ids, true));
        }

        public static string GetNormalizedName(IList<Identifier> ids, bool tolower)
        {
            StringBuilder builder = new StringBuilder();
            foreach (Identifier id in ids)
            {
                string name = id.Value;
                if (!String.IsNullOrEmpty(name))
                {
                    if (builder.Length > 0)
                        builder.Append(".");
                    builder.Append(tolower ? name.ToLower() : name);
                }
            }
            return builder.ToString();
        }

        public static string GetCursorName(CursorId cursorId)
        {
            if (cursorId.Name is Identifier)
                return ((Identifier)cursorId.Name).Value;
            if (cursorId.Name is Literal)
                return ((Literal)cursorId.Name).Value;

            string name = null;
            for (int i = cursorId.FirstTokenIndex; i <= cursorId.LastTokenIndex; i++)
            {
                TSqlParserToken token = cursorId.ScriptTokenStream[i];
                if (token.TokenType == TSqlTokenType.Variable || token.TokenType == TSqlTokenType.Identifier)
                {
                    name = token.Text;
                    break;
                }
            }
            return name;
        }

        public static SelectionRectangle GetOptionClauseBounds(TSqlStatement statement)
        {   // workaround bug that parsed object does not contain valid location information

            TSqlParserToken optionStartToken = null;
            TSqlParserToken optionEndToken = null;
            Stack<TSqlParserToken> happyEndings = new Stack<TSqlParserToken>();

            // search backwards for the option token
            for (int i = statement.LastTokenIndex; i >= statement.FirstTokenIndex; i--)
            {
                TSqlParserToken token = statement.ScriptTokenStream[i];
                if (token.TokenType == TSqlTokenType.LeftParenthesis)
                {
                    if (happyEndings.Count > 0)
                        optionEndToken = happyEndings.Pop();
                    else
                        optionEndToken = null;
                    continue;
                }
                if (token.TokenType == TSqlTokenType.RightParenthesis)
                {
                    happyEndings.Push(token);
                    continue;
                }
                if (token.TokenType == TSqlTokenType.Option)
                {
                    optionStartToken = token;
                    break;
                }
            }

            if (optionStartToken != null)
            {
                int endOffset = optionEndToken != null ? optionEndToken.Offset : statement.ScriptTokenStream[statement.LastTokenIndex - 1].Offset;
                int length = endOffset - optionStartToken.Offset;
                return new SelectionRectangle(optionStartToken.Offset, optionStartToken.Line, optionStartToken.Column, length);
            }

            return null;
        }

        public static bool Contains(this TSqlFragment owner, TSqlFragment child)
        {
            if (child.FirstTokenIndex >= owner.FirstTokenIndex)
            {
                return (child.LastTokenIndex <= owner.LastTokenIndex);
            }
            return false;
        }

        public static SelectionRectangle GetJoinHintBounds(this QualifiedJoin join)
        {
            if (join.JoinHint != JoinHint.None)
            {
                // search for an identifier token with hintText
                string hintText = join.JoinHint.ToString();

                bool scan = false;
                for (int i = join.FirstTokenIndex; i < join.LastTokenIndex; i++)
                {
                    TSqlParserToken token = join.ScriptTokenStream[i];
                    switch (token.TokenType)
                    {
                        case TSqlTokenType.Inner:
                        case TSqlTokenType.Outer:
                        case TSqlTokenType.Left:
                        case TSqlTokenType.Right:
                        case TSqlTokenType.Full:
                            scan = true;
                            break;
                        case TSqlTokenType.Identifier:
                            if (scan)
                            {
                                if (token.Text.Equals(hintText, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    return token.GetSelectionBounds();
                                }
                            }
                            break;
                        case TSqlTokenType.Join:
                            return null;
                    }
                }
            }
            return null;
        }

        #endregion



    }
}
