using Romanesco.DataModel.Entities.Component;
using Romanesco.DataModel.Factories;
using Romanesco.DataModel.Test.Structural;
using RomanescoPlus.Annotations;

namespace Romanesco.DataModel.Test;

internal class NullCommandObserver : IEditorCommandObserver
{
    public void RunCommand(EditorCommand command)
    {
    }
}

internal class CustomCommandTest
{
    private NullCommandObserver? _commandObserver;
    private AggregatedFactory? _aggregatedFactory;

    [SetUp]
    public void Setup()
    {
        _commandObserver = new NullCommandObserver();
        _aggregatedFactory = new AggregatedFactory()
        {
            ClassFactory = new ClassFactory()
            {
                CommandObserver = _commandObserver
            },
            Factories = new IModelFactory[]
            {
                new PrimitiveFactory(),
                new ArrayFactory()
            }
        };
    }

    [Test]
    public void 引数と戻り値とプロパティの型が一致しているとコマンドとして認められる()
    {
        var model = _aggregatedFactory!.LoadType("Array", typeof(int[]), _aggregatedFactory);
        var commands = EditorCommand.ExtractCommands(
            typeof(ValidCommandSubject),
            typeof(ValidCommandSubject).GetProperty("Array")!,
            model!,
            _commandObserver!);

        using var sequence = commands.BeginAssertion()
            .NotNull()
            .Sequence(x => x);

        sequence.Next()
            .AreEqual(nameof(ValidCommandSubject.Command), x => x.MethodName);
    }

    [Test]
    public void Staticでないメソッドはコマンドとして認められない()
    {
        var model = _aggregatedFactory!.LoadType("Array", typeof(int[]), _aggregatedFactory);
        var commands = EditorCommand.ExtractCommands(
            typeof(NonStaticCommandSubject),
            typeof(NonStaticCommandSubject).GetProperty("Array")!,
            model!,
            _commandObserver!);

        using var sequence = commands.BeginAssertion()
            .NotNull()
            .Sequence(x => x);
    }

    [Test]
    public void Privateなメソッドはコマンドとして認められない()
    {
        var model = _aggregatedFactory!.LoadType("Array", typeof(int[]), _aggregatedFactory);
        var commands = EditorCommand.ExtractCommands(
            typeof(PrivateCommandSubject),
            typeof(PrivateCommandSubject).GetProperty("Array")!,
            model!,
            _commandObserver!);

        using var sequence = commands.BeginAssertion()
            .NotNull()
            .Sequence(x => x);
    }

    [Test]
    public void 引数の型が一致していないメソッドはコマンドとして認められない()
    {
        var model = _aggregatedFactory!.LoadType("Array", typeof(int[]), _aggregatedFactory);
        var commands = EditorCommand.ExtractCommands(
            typeof(InvalidParameterSubject),
            typeof(InvalidParameterSubject).GetProperty("Array")!,
            model!, _commandObserver!);

        using var sequence = commands.BeginAssertion()
            .NotNull()
            .Sequence(x => x);
    }

    [Test]
    public void 引数が2個のメソッドはコマンドとして認められない()
    {
        var model = _aggregatedFactory!.LoadType("Array", typeof(int[]), _aggregatedFactory);
        var commands = EditorCommand.ExtractCommands(
            typeof(UnnecessaryParameterSubject),
            typeof(UnnecessaryParameterSubject).GetProperty("Array")!,
            model!, _commandObserver!);

        using var sequence = commands.BeginAssertion()
            .NotNull()
            .Sequence(x => x);
    }

    [Test]
    public void 戻り値の型が一致していないメソッドはコマンドとして認められない()
    {
        var model = _aggregatedFactory!.LoadType("Array", typeof(int[]), _aggregatedFactory);
        var commands = EditorCommand.ExtractCommands(
            typeof(InvalidReturnSubject),
            typeof(InvalidReturnSubject).GetProperty("Array")!,
            model!, _commandObserver!);

        using var sequence = commands.BeginAssertion()
            .NotNull()
            .Sequence(x => x);
    }

    [Test]
    public void プロパティの型が一致していないメソッドはコマンドとして認められない()
    {
        var model = _aggregatedFactory!.LoadType("Array", typeof(int[]), _aggregatedFactory);
        var commands = EditorCommand.ExtractCommands(
            typeof(InvalidPropertyTypeSubject),
            typeof(InvalidPropertyTypeSubject).GetProperty("Array")!,
            model!, _commandObserver!);

        using var sequence = commands.BeginAssertion()
            .NotNull()
            .Sequence(x => x);
    }

    private class ValidCommandSubject
    {
        [EditorCommandTarget(nameof(Command))]
        public int[] Array { get; set; }

        public static int[] Command(int[] array)
        {
            return array.Select(x => x * 2).ToArray();
        }
    }

    private class NonStaticCommandSubject
    {
        [EditorCommandTarget(nameof(Command))]
        public int[] Array { get; set; }

        public int[] Command(int[] array)
        {
            return array.Select(x => x * 2).ToArray();
        }
    }
    
    private class PrivateCommandSubject
    {
        [EditorCommandTarget(nameof(Command))]
        public int[] Array { get; set; }

        private static int[] Command(int[] array)
        {
            return array.Select(x => x * 2).ToArray();
        }
    }

    private class InvalidParameterSubject
    {
        [EditorCommandTarget(nameof(Command))]
        public int[] Array { get; set; }

        public static int[] Command(string[] array)
        {
            return array.Select(x => 1).ToArray();
        }
    }

    private class UnnecessaryParameterSubject
    {
        [EditorCommandTarget(nameof(Command))]
        public int[] Array { get; set; }

        public static int[] Command(int[] array, int[] array2)
        {
            return array.Select(x => 1).ToArray();
        }
    }

    private class InvalidReturnSubject
    {
        [EditorCommandTarget(nameof(Command))]
        public int[] Array { get; set; }

        public static string[] Command(int[] array)
        {
            return array.Select(x => x.ToString()).ToArray();
        }
    }

    private class InvalidPropertyTypeSubject
    {
        [EditorCommandTarget(nameof(Command))]
        public string[] Array { get; set; }

        public static int[] Command(int[] array)
        {
            return array.Select(x => x * 2).ToArray();
        }
    }
}