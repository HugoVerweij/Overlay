# Overlay
![Overlay_nRRQ3rZnsb](https://github.com/HugoVerweij/Overlay/assets/163334632/927cbc3f-0895-4f47-a1ad-a46308f60e40)

## Wat is Overlay precies?
Overlay is een WPF applicatie dat zich 'over' het gehele scherm projecteert, waardoor de gebruiker gemakkelijk widgets kan aanroepen. Hier kan men bijvoorbeeld denken aan notities, clock, date & time, calculator, notifications, integratie met andere persoonlijke projecten, etc.

Overlay maakt gebruik van polymorfie en reflection om alle widgets in te laden.

```csharp
// Invoke on the new thread.
Dispatcher.BeginInvoke(new Action(async () =>
{
	// Load the states form the xml file.
	List<OverlayWindowState> saved = await LoadHelper.LoadWidgetsAsync();

	// Crawl through the assembly and fetch every assignable class from the base class.
	foreach (Type type in Extentions.GetTypesInAssembly(Assembly.GetExecutingAssembly(), typeof(OverlayWindowBase)))
	{
		// Relevante code.
	}
}), DispatcherPriority.ContextIdle);
```

# Demonstratie
https://github.com/HugoVerweij/Overlay/assets/163334632/dbe9009e-b854-40ae-9209-bc1bbe07563a
