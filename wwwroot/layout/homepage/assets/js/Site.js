//$(function () {
//    $('body').on('click', 'a[data-toggle="tab"]', function (e) {
//        e.preventDefault();
//        var target = $(this).attr('href');
//        if ($(target).length) { // Ensure the target exists
//            $('.tab-pane').removeClass('active show');
//            $(target).addClass('active show');
//            $('.nav-link').removeClass('active');
//            $(this).addClass('active');
//        }
//    });
//});


function addToCart(event, productId) {
    event.preventDefault();
    const button = $(event.target); // Reference the clicked button
    button.prop('disabled', true).text('Adding...'); // Disable button and show loading state

    $.ajax({
        url: '/Cart/AddToCart',
        method: 'POST',
        data: { productId, quantity: 1 },
        success: () => {
            alert('Added to cart!');
        },
        error: () => {
            alert('Error adding to cart.');
        },
        complete: () => {
            button.prop('disabled', false).text('Add to Cart'); // Re-enable button
        },
    });
}
