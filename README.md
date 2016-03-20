## Description

This tool strips boilerplate text from the tops of files (which we refer to as "preamble"). This tool may also be used with many files (even specified via filename patterns) which are then concatenated. The output is printed to standard out.


## Basic Concept

A preamble is defined by a set of states (referred to, collectively, as a "matcher"). A state is defined as a set of regular-expressions (and more minor parameters). If we can match a certain number of lines to at least one of the regular expressions in each state, in order, then we will have succeeded in identifying that there exists a preamble and it will be removed.


## Examples

### Practical

This example is essentially taken from the *CommonMatchers.cs* file. This file has a class that statically describes some sample preambles for convenience. Feel free to submits pull-requests with matchers describing other common preambles.

This is what we want to remove from the tops of our files:

```
/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
			   SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
```

This is the matcher that identifies it:

```csharp
PreambleMatcher VisualStudioSql = new PreambleMatcher(
	new List<PreambleState>() {
		// Open comment
		new PreambleState(new List<string>() { @"^/\*$" }, 1, 1),

        // Title
		new PreambleState(new List<string>() { @"^(Pre|Post)\-Deployment Script Template$" }, 1, 1),
		
		// Top border (dashes)
		new PreambleState(new List<string>() { @"^\-+$" }, 1, 1),

		// Body content (any line that doesn't start with a dash).
		new PreambleState(new List<string>() { @"^[^-]" }, 1, null),

		// Bottom border (dashes)
		new PreambleState(new List<string>() { @"^\-+$" }, 1, 1),

		// Close comment
		new PreambleState(new List<string>() { @"^\*/$" }, 1, 1),

		// The following blank line
		new PreambleState(new List<string>() { @"^$" }, 0, 1)
	}
);
```

For each successive state that identifies each successive part of the preamble, we provide:

1. An list of regular-expression strings. One of them must match the current-line in order to progress (OR-relationship).
2. A nullable minimum and maximum number of lines.


### Simple

A simpler example would be:

```
=========
Some text
=========
```

The matcher could then be:

```
PreambleMatcher SimpleBlock = new PreambleMatcher(
	new List<PreambleState>() {
		new PreambleState(new List<string>() { @"^=+$" }, 1, 1),
		new PreambleState(new List<string>() { @"^[^=\-]" }, 1, null),
		new PreambleState(new List<string>() { @"^=+$" }, 1, 1),
		new PreambleState(new List<string>() { @"^$" }, 0, 1)
	}
);
```


## Console Tool

A console-tool project is included. The common-matcher class has a dictionary that maps all of its matchers to simple but description strings. Using the tool, you pass the name of a common-matcher, a separator prefix string (inserted between files and concatenated with the name of the file being added below it, in the output), and at least one filepath/pattern. The stripped, concatenated output is printed.


## Building

Three are three ways to acquire/build StripPreamble:

1. Install the library via [NuGet](https://www.nuget.org/packages/StripPreamble) and implement it from your own application/tool.
2. Clone the [whole project](https://github.com/dsoprea/StripPreambleCs) (which includes both the assembly and the command-line tool) and open and build the solution from the root of the project.
3. Clone the [whole project](https://github.com/dsoprea/StripPreambleCs) (or download a zip-archive), open the console solution via the solution in the console-project's directory, and allow it to download the assembly via NuGet.
4. Build the assembly directly using the solution in the assembly directory. 

(2) is suggested if you just want to use it as a simple console tool.


## Usage

### Via code

```csharp
Utility u = new Utility();
IList<string> lines = u.GetLinesFromText(body, 10);

CommonMatchers.SimpleBlock.TryStrip(lines);
string output = String.Join(Environment.NewLine, lines);
```

### Via console

```
C:\code\local\StripPreamble\StripPreambleCommand\bin\Debug>StripPreambleCommand.exe vs_sql "-- FILE: " ..\..\Test\SqlFile1.sql ..\..\Test\SqlFile2.sql
Reading: [C:\code\local\StripPreamble\StripPreambleCommand\Test\SqlFile1.sql]
Reading: [C:\code\local\StripPreamble\StripPreambleCommand\Test\SqlFile2.sql]
-- FILE: C:\code\local\StripPreamble\StripPreambleCommand\Test\SqlFile1.sql


PRE DEPLOY SQL CONTENT 1


-- FILE: C:\code\local\StripPreamble\StripPreambleCommand\Test\SqlFile2.sql


POST DEPLOY SQL CONTENT 2



```

The lines beginning with "Reading:" are printed to STDERR but the file-content (below it) is printed to STDOUT.


## Tweaking/Debugging

The matcher has two properties that you might find useful to set:

`Debug`: Whether to print hit/miss information to the debugger console. FALSE by default.
`StripLines`: Whether to automatically strip every line before checking. TRUE by default.
