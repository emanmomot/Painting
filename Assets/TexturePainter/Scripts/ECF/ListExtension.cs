
using System;
using System.Collections.Generic;

public static class ListExtension {
	public static List<T> Splice<T>(this List<T> source,int index,int count)
	{
		var items = source.GetRange(index, count);
		source.RemoveRange(index,count);
		return items;
	}
}