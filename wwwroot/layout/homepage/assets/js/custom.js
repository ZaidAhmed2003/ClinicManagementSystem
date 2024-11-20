// ==================================================
// Project Name  :  Medicia - medical eCommerce shop
// File          :  JS Base
// Version       :  1.0.0
// Last change   :  28 April 2021
// Author        :  Marvel Theme (https://themeforest.net/user/marvel_theme)
// ==================================================


(function($) {
  "use strict";


  // back to top - start
  // --------------------------------------------------
  $(window).scroll(function() {
    if ($(this).scrollTop() > 200) {
      $('#backtotop:hidden').stop(true, true).fadeIn();
    } else {
      $('#backtotop').stop(true, true).fadeOut();
    }
  });
  $(function() {
    $("#scroll").on('click', function() {
      $("html,body").animate({
        scrollTop: $("#thetop").offset().top
      }, "slow");
      return false
    })
  });
  // back to top - end
  // --------------------------------------------------


  // preloader - start
  // --------------------------------------------------
  // $(window).on('load', function(){
  //   $('#preloader').fadeOut('slow',function(){$(this).remove();});
  // });
  // preloader - end
  // --------------------------------------------------


  // search box - start
  // --------------------------------------------------
  $('.main-search-btn').on('click', function() {
    $('.main-search-btn > i').toggleClass('la-times');
  });
  // search box - end
  // --------------------------------------------------


  // tooltip - start
  // --------------------------------------------------
  $('[data-toggle="tooltip"]').tooltip();
  $('.tooltipes').tooltip();
  // tooltip - end
  // --------------------------------------------------


  // background image - start
  // --------------------------------------------------
  $('[data-background]').each(function() {
    $(this).css('background-image', 'url('+ $(this).attr('data-background') + ')');
  });
  // background image - end
  // --------------------------------------------------


  // menu button - start
  // --------------------------------------------------
  $(document).ready(function () {
    $('.close-btn, .overlay').on('click', function () {
      $('#sidebar-menu').removeClass('active');
      $('.overlay').removeClass('active');
    });

    $('.mobile-menu-btn, .mobile-btn-cart').on('click', function () {
      $('#sidebar-menu').addClass('active');
      $('.overlay').addClass('active');
    });
  });
  // menu button - end
  // --------------------------------------------------


  // 3rd level dropdown menu - start
  // --------------------------------------------------
  $('.menu_list .dropdown').on('show.bs.dropdown', function(e){
    $(this).find('.dropdown-menu').first().stop(true, true).slideDown(400);
  });
  $('.menu_list .dropdown').on('hide.bs.dropdown', function(e){
    $(this).find('.dropdown-menu').first().stop(true, true).slideUp(500);
  });
  // 3rd level dropdown menu - end
  // --------------------------------------------------


  // sticky header - start
  // --------------------------------------------------
  $(window).on('scroll', function () {
    if ($(this).scrollTop() > 150) {
      $('.sticky-header').addClass("stuck")
    } else {
      $('.sticky-header').removeClass("stuck")
    }
  });


  $(window).on('scroll', function () {
    if ($(this).scrollTop() > 150) {
      $('.header-section').addClass("stuck")
    } else {
      $('.header-section').removeClass("stuck")
    }
  });
  // sticky header - end
  // --------------------------------------------------


  // popup image - start
  // --------------------------------------------------
  $('.zoom-gallery').magnificPopup({
    delegate: '.popup-image',
    type: 'image',
    closeOnContentClick: false,
    closeBtnInside: false,
    mainClass: 'mfp-with-zoom mfp-img-mobile',
    image: {
      verticalFit: true,
      titleSrc: function(item) {
        return item.el.attr('title') + ' &middot; <a class="image-source-link" href="'+item.el.attr('data-source')+'" target="_blank">image source</a>';
      }
    },
    gallery: {
      enabled: true
    },
    zoom: {
      enabled: true,
      duration: 300,
      opener: function(element) {
        return element.find('img');
      }
    }
  });
  // popup image - end
  // --------------------------------------------------


  // popup video - start
  // --------------------------------------------------
  $('.play-btn').magnificPopup({
    // disableOn: 700,
    type: 'iframe',
    preloader: false,
    removalDelay: 160,
    mainClass: 'mfp-fade',
    fixedContentPos: false
  });
  // popup video - end
  // --------------------------------------------------


  // main carousel - start
  // --------------------------------------------------
  $('#main-carousel').owlCarousel({
    items:1,
    margin:0,
    nav:true,
    loop:true,
    dots:true,
    autoplay:true,
    smartSpeed:1000,
    autoplayTimeout:6000,
    autoplayHoverPause:true
  });
  // main carousel - end
  // --------------------------------------------------


  // column 4 carousel - start
  // --------------------------------------------------
  $('.column-4-carousel').owlCarousel({
    nav:true,
    loop:true,
    margin:30,
    dots:false,
    autoplay:true,
    smartSpeed:1000,
    autoplayTimeout:6000,
    autoplayHoverPause:true,
    responsive:{
      0:{
        items:1
      },
      480:{
        items:2
      },
      700:{
        items:3
      },
      1000:{
        items:4
      }
    }
  });

  $('.column-3-carousel').owlCarousel({
    nav:false,
    loop:true,
    margin:0,
    dots:false,
    smartSpeed:1000,
    responsive:{
      0:{
        items:1
      },
      600:{
        items:3
      },
      1000:{
        items:3
      }
    }
  });
  // column 4 carousel - end
  // --------------------------------------------------


  // sidebar deals carousel - start
  // --------------------------------------------------
  $('#sidebar-deals-carousel').owlCarousel({
    items:1,
    nav:true,
    margin:15,
    loop:true,
    dots:false,
    autoplay:true,
    smartSpeed:1000,
    autoplayTimeout:6000,
    autoplayHoverPause:true
  });
  // sidebar deals carousel - end
  // --------------------------------------------------


  // details carousel - start
  // --------------------------------------------------
  $('.details-image-carousel').owlCarousel({
    items:1,
    nav:false,
    dots:false,
    thumbs:true,
    smartSpeed:500,
    thumbImage:false,
    thumbItemClass:'item',
    thumbsPrerendered:true,
    thumbContainerClass:'owl-thumbs'
  });
  // details carousel - end
  // --------------------------------------------------


  // image carousel - start
  // --------------------------------------------------
  $('#image-carousel').owlCarousel({
    items:1,
    nav:true,
    margin:15,
    loop:true,
    dots:false,
    autoplay:true,
    smartSpeed:1000,
    autoplayTimeout:6000,
    autoplayHoverPause:true
  });
  // image carousel - end
  // --------------------------------------------------


  // testimonial carousel - start
  // --------------------------------------------------
  $('.testimonial-carousel').owlCarousel({
    nav:false,
    loop:true,
    margin:30,
    dots:true,
    autoplay:true,
    smartSpeed:600,
    autoplayTimeout:6000,
    autoplayHoverPause:true,
    responsive:{
      0:{
        items:1
      },
      767:{
        items:2
      },
      1024:{
        items:3
      }
    }
  });
  // testimonial carousel - end
  // --------------------------------------------------


  // brand carousel - start
  // --------------------------------------------------
  $('.brand-carousel').owlCarousel({
    margin:0,
    nav:false,
    loop:true,
    dots:false,
    autoplay:true,
    smartSpeed:800,
    autoplayTimeout:4000,
    responsive:{
      0:{
        items:2
      },
      300:{
        items:3
      },
      700:{
        items:4
      },
      1024:{
        items:6
      }
    }
  });
  // brand carousel - end
  // --------------------------------------------------


  // price range - start
  // --------------------------------------------------
  if($("#slider-range").length){
    $("#slider-range").slider({
      range: true,
      min: 5,
      max: 1000,
      values: [ 30.00, 429.00 ],
      slide: function( event, ui ) {
        $( "#amount" ).val( "$" + ui.values[ 0 ] + " - $" + ui.values[ 1 ] );
      }
    });
    $( "#amount" ).val( "$" + $( "#slider-range" ).slider( "values", 0 ) +
      " - $" + $( "#slider-range" ).slider( "values", 1 ) );
  }

  $('.ar_top').on('click', function () {
    var getID = $(this).next().attr('id');
    var result = document.getElementById(getID);
    var qty = result.value;
    $('.proceed_to_checkout .update-cart').removeAttr('disabled');
    if( !isNaN( qty ) ) {
      result.value++;
    }else{
      return false;
    }
  });
  // price range - end
  // --------------------------------------------------


  // isotope masonry & filtering - start
  // --------------------------------------------------
  var $grid = $('.masonry-grid').imagesLoaded( function() {
    $grid.masonry({
      itemSelector: '.grid-item',
      percentPosition: true,
      columnWidth: '.grid-sizer'
    }); 
  });

  function portfolioMasonry() {
    var portfolio = $(".element-grid");
    if (portfolio.length) {
      portfolio.imagesLoaded(function () {
        portfolio.isotope({
          itemSelector: ".element-item",
          layoutMode: 'masonry',
          filter: "*",
          animationOptions: {
            duration: 1000
          },
          transitionDuration: '0.5s',
          masonry: {

          }
        });

        $(".filter-btns-group > ul > li > button").on('click', function () {
          $(".filter-btns-group > ul > li > button").removeClass("active");
          $(this).addClass("active");

          var selector = $(this).attr("data-filter");
          portfolio.isotope({
            filter: selector,
            animationOptions: {
              animationDuration: 750,
              easing: 'linear',
              queue: false
            }
          })
          return false;
        })
      });
    }
  }
  portfolioMasonry();
  // isotope masonry & filtering - end
  // --------------------------------------------------


  // multy count down - start
  // --------------------------------------------------
  $('.countdown-list').each(function(){
    $('[data-countdown]').each(function() {
      var $this = $(this), finalDate = $(this).data('countdown');
      $this.countdown(finalDate, function(event) {
        var $this = $(this).html(event.strftime(''
          + '<li class="timer-item days"><strong>%D</strong><small>days</small></li>'
          + '<li class="timer-item hours"><strong>%H</strong><small>hours</small></li>'
          + '<li class="timer-item mins"><strong>%M</strong><small>minutes</small></li>'
          + '<li class="timer-item seco"><strong>%S</strong><small>seconds</small></li>'));
      });
    });
  });
  // multy count down - end
  // --------------------------------------------------


  // quantity - start
  // --------------------------------------------------
  (function() {
    window.inputNumber = function(el) {
      var min = el.attr("min") || false;
      var max = el.attr("max") || false;

      var els = {};

      els.dec = el.prev();
      els.inc = el.next();

      el.each(function() {
        init($(this));
      });

      function init(el) {
        els.dec.on("click", decrement);
        els.inc.on("click", increment);

        function decrement() {
          var value = el[0].value;
          value--;
          if (!min || value >= min) {
            el[0].value = value;
          }
        }

        function increment() {
          var value = el[0].value;
          value++;
          if (!max || value <= max) {
            el[0].value = value++;
          }
        }
      }
    };
  })();
  inputNumber($(".input-number-1"));
  inputNumber($(".input-number-2"));
  inputNumber($(".input-number-3"));
  // quantity - end
  // --------------------------------------------------


  // google map - start
  // --------------------------------------------------
  if ( $('#mapBox').length ){
    var $lat = $('#mapBox').data('lat');
    var $lon = $('#mapBox').data('lon');
    var $zoom = $('#mapBox').data('zoom');
    var $marker = $('#mapBox').data('marker');
    var $info = $('#mapBox').data('info');
    var $markerLat = $('#mapBox').data('mlat');
    var $markerLon = $('#mapBox').data('mlon');
    var map = new GMaps({
      el: '#mapBox',
      lat: $lat,
      lng: $lon,
      scrollwheel: false,
      scaleControl: true,
      streetViewControl: false,
      panControl: true,
      disableDoubleClickZoom: true,
      mapTypeControl: false,
      zoom: $zoom,
    });
    map.addMarker({
      lat: $markerLat,
      lng: $markerLon,
      icon: $marker,    
      infoWindow: {
        content: $info
      }
    })
  }
  // google map - end
  // --------------------------------------------------


})(jQuery);