# Help view

## Purpose
The user interface is similar to the interface of other video games and generally does not require too much documentation. However, the game of Go itself would merit from documentation, since its rules are not readily apparent.

Therefore, we focused the help contents mostly on text about the board game, including additional information for players who might be interested in details (e.g. the history). This is part of our aim to be "the one app" for a Go player. 

## Implementation

Because the introduction to Go needed board illustrations to be useful, we had to find a way to include not only font markup but also bitmaps in the help pages. We chose to go with HTML pages because they offer more possibilities than Markup files and there is a built-in control that displays them.

The pages are loaded from HTML files as strings, including base64-encoded images. This is okay because the images are small, and encoding them inside the file itself makes the help system more robust (loading additional files from the WebView inside UWP apps is somewhat messy).

## Not multilingual
We chose not to translate the help pages in Czech because that would take too much effort. We already spent significant time to write (and edit) the English pages. Translation of these would be exhausting.