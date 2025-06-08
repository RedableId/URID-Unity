**URID** - is an **U**niversal **R**eadable **Id**entifier packed into 128/64-bit vector.

Priorities of concept:
* Tightly fit as more text as possible into bit vector.
* Support indices for identifiyng of array items.
* Each ID can be encoded into bit vector only one way
* Each bit vector is correct and encodes some Id
* Format URID in different styles to match a project requirements (PascalCase, camelCase, snake_case, cebab-case, other case...)
* Sorting of bit vector by value sorts Ids in alphabetical order (at least sub words of Id)

# Encoding scheme

URID encodes a string (identifier) and, optionally, an index into a compact N-bit vector (typically 64 or 128 bits). Here is how it works:

## 1. Splitting into words
The input string is split into words—sequences of Latin letters (A-Z, a-z). Separators (such as spaces, underscores, hyphens) are not encoded, but can be restored during formatting.

## 2. Word encoding
- For each word, its length (number of letters) is encoded as a prefix. The number of bits for the length depends on the remaining bits in the vector.
- The word itself is encoded as a number in base-26 (alphabet size: a=0, b=1, ..., z=25).
- All letters of the word are packed into a single number, with the least significant digits representing the first letters.

## 3. Prefixes and packing
- The prefix (word length) and the encoded word are packed sequentially into the bit vector.
- If there are unused bits left, they can be used to store an index (for example, to identify an array element).

## 4. Index encoding
- If the identifier contains an index (e.g., `Item42`), the index is encoded into the remaining free bits of the vector after all words are encoded.
- The index is stored in the least significant bits of the URID and can be restored during decoding.

## 5. Decoding
- Prefixes (word lengths) and encoded words are extracted from the bit vector in order.
- For each word, letters are restored by repeatedly taking modulo 26 (in reverse order, then reversed back).
- If there are unused bits left, they are decoded as the index.

## 6. Sorting
- Sorting by the value of the bit vector corresponds to the alphabetical order of identifiers (at least for subwords).

# Example

Suppose we have the identifier `UniversalReadableId_42`:
- Words: `Universal`, `Readable`, `Id`
- Index: `42`
- Each word is encoded as a base-26 number with a length prefix.
- Index 42 is encoded into the remaining bits.
- Everything is packed together into 64 or 128 bits.

// TODO: detailed example