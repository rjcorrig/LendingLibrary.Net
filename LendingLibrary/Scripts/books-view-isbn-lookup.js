// Requires Scripts/lendinglibrary.js, built from NodeScripts/index.js using npm webpack
// See README.md

$("#ISBNLookup").on("click", function (e) {
    lendingLibrary.isbn.resolve($("#ISBN").val(), function (err, book) {
        if (err) {
            console.log(err);
        } else {
            $("#Title").val(book.title);
            $("#Author").val(book.authors.join(", "));
            $("#Genre").val(book.categories[0]);
        }
    });
});
