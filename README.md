# Baksteen.Extensions.DeepSize
C# extension method that measures the size of an object and everything contained or referenced by it. It's useful when you're trying to minimize the memory footprint of a class during development, or to examine which objects are getting bloated in your production software.
The calculated size is approximate because it does not account for the internal memory representation that may differ slightly due to field alignment padding. 
