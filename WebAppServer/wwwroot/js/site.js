function setVideo(event) {

	$('#' + event.data.param2).attr("class", "hide-imgRechercher");
	$(".imgRechercher").remove();

	$.ajax({
		url: "/api/Settings/" + event.data.param2,
		type: "POST",
		data: JSON.stringify(event.data.param1),
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: function (data) {

			var parent = $('#' + event.data.param2).parent();

			parent.find("#description").text(data.description);
			parent.find("#title").text(data.title);
			parent.find("#imageVideo").attr("src", "https://image.tmdb.org/t/p/w370_and_h556_bestv2" + data.poster);

			$('#myModal').modal('toggle');
		},
		error: function (xhr, ajaxOptions, thrownError) {
			console.log("Error." + thrownError);
		}
	});
}

function getVideo(idVideo) {

	$('.result-search-video').remove();

	$.ajax({
		url: "/api/Settings/" + idVideo,
		type: "GET",
		success: function (data) {

			$("#fileName span").text(data.movieInformation.fileName);
			$("#titrevideo").val(data.movieInformation.titre);
			$("#idBtnSearchVideo").attr("value", idVideo);
		},
		error: function (xhr, ajaxOptions, thrownError) {
			alert(xhr.status);
			alert(thrownError);
		}
	});
}

function addMovieToWishList(idVideo) {

	$.ajax({
		type: "POST",
		url: "/Wish/FindWish?handler=AddWishMovie",
		data: { "idMovie": idVideo },
		headers: {
			RequestVerificationToken:
				$('input:hidden[name="__RequestVerificationToken"]').val()
		},
		success: function (data) {
			$("#id_" + idVideo).text("Ajouté");
			$("#id_" + idVideo).attr("class", "btn btn-success");
			$("#id_" + idVideo).attr("onclick", "removeMovieToWishList(" + idVideo + ")");
		},
		error: function (xhr, ajaxOptions, thrownError) {
			alert(xhr.status);
			alert(thrownError);
		}
	});
}

function removeMovieToWishList(idVideo) {

	$.ajax({
		type: "POST",
		url: "/Wish/FindWish?handler=RemoveWishMovie",
		data: { "idMovie": idVideo },
		headers: {
			RequestVerificationToken:
				$('input:hidden[name="__RequestVerificationToken"]').val()
		},
		success: function (data) {
			$("#id_" + idVideo).text("Souhait");
			$("#id_" + idVideo).attr("class", "btn btn-primary");
			$("#id_" + idVideo).attr("onclick", "addMovieToWishList(" + idVideo + ")");
		},
		error: function (xhr, ajaxOptions, thrownError) {
			alert(xhr.status);
			alert(thrownError);
		}
	});
}

function getSearchVideos() {

	$('.result-search-video').remove();

	$.ajax({
		url: "/api/Settings/search",
		type: "POST",
		data: JSON.stringify($("#titrevideo").val()),
		contentType: "application/json",
		dataType: "json",
		success: function (datas) {

			$.each(datas, function (index, value) {

				var image = document.createElement("img");
				image.setAttribute("src", value.urlAffiche);
				image.setAttribute("class", "imgRechercher");
				image.setAttribute("id", value.idVideoTmDb);

				var divTitre = document.createElement("div");
				divTitre.textContent = value.titre;
				divTitre.setAttribute("class", "text-center text-bold");

				var div = document.createElement("div");
				div.setAttribute("class", "result-search-video");

				div.appendChild(image);
				div.appendChild(divTitre);

				$('#resultSearchVideo').append(div);
				$('#' + value.idVideoTmDb).click({ param1: value.idVideoTmDb, param2: $("#idBtnSearchVideo").attr("value") }, setVideo);
			});

		},
		error: function (xhr, ajaxOptions, thrownError) {
			console.log("Error." + thrownError);
		}
	});
}

function getMoviesByName() {

	$('.result-search-video').remove();

	$.ajax({
		url: "/api/Settings/search",
		type: "POST",
		data: JSON.stringify($("#titrevideo").val()),
		contentType: "application/json",
		dataType: "json",
		success: function (datas) {

			$.each(datas, function (index, value) {

				var image = document.createElement("img");
				image.setAttribute("src", value.urlAffiche);
				image.setAttribute("class", "imgRechercher");
				image.setAttribute("id", value.idVideoTmDb);

				var divTitre = document.createElement("div");
				divTitre.textContent = value.titre;
				divTitre.setAttribute("class", "text-center text-bold");

				var div = document.createElement("div");
				div.setAttribute("class", "result-search-video");

				div.appendChild(image);
				div.appendChild(divTitre);

				$('#resultSearchVideo').append(div);
				$('#' + value.idVideoTmDb).click({ param1: value.idVideoTmDb, param2: $("#idBtnSearchVideo").attr("value") }, setVideo);
			});

		},
		error: function (xhr, ajaxOptions, thrownError) {
			console.log("Error." + thrownError);
		}
	});
}
