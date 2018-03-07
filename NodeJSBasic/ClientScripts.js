var socket = io(); //load socket.io-client and connect to the host that serves the page
var theField = document.getElementById("playingfield");
var ctx = theField.getContext("2d");
theField.addEventListener('touchstart', onTouchStart, false);
theField.addEventListener('touchmove', onTouchMove, false);
var bMouseDown = false;
function Coords() {
    var x = 0;
    var y = 0;
}

function ChangeChecks(black, blue, red, green)
{
    document.getElementById("blackCheck").checked = black;
    document.getElementById("blueCheck").checked = blue;
    document.getElementById("redCheck").checked = red;
    document.getElementById("greenCheck").checked = green;
}

window.addEventListener("load", function () { //when page loads
    socket.emit('refresh', 'rectlist');
    document.getElementById("blackCheck").addEventListener("change", function () { //add event listener for when checkbox changes
        socket.emit("blackChk", Number(this.checked)); //send button status to server (as 1 or 0)
    });
    document.getElementById("blueCheck").addEventListener("change", function () { //add event listener for when checkbox changes
        socket.emit("blueChk", Number(this.checked)); //send button status to server (as 1 or 0)
    });
    document.getElementById("redCheck").addEventListener("change", function () { //add event listener for when checkbox changes
        socket.emit("redChk", Number(this.checked)); //send button status to server (as 1 or 0)
    });
    document.getElementById("greenCheck").addEventListener("change", function () { //add event listener for when checkbox changes
        socket.emit("greenChk", Number(this.checked)); //send button status to server (as 1 or 0)
    });
});

socket.on('color', function (data) { //get button status from client
    switch (data) {
        case 1: ChangeChecks(1, 0, 0, 0); break;
        case 2: ChangeChecks(0, 1, 0, 0); break;
        case 3: ChangeChecks(0, 0, 1, 0); break;
        case 4: ChangeChecks(0, 0, 0, 1); break;
    }
    
});
socket.on('info', function (data) {
    document.getElementById("information").innerHTML = data;
})
socket.on('rect', function (data) {
    document.getElementById("information").innerHTML = data.x + " " + data.y;
    ctx.fillStyle = "rgb(" + data.r + ", " + data.g + ", " + data.b + ")";
    ctx.fillRect(data.x - data.w / 2, data.y - data.h / 2, data.w, data.h);
})
socket.on('rectlistdata', function (data) {
    document.getElementById("information").innerHTML = "Received UpTo Date Data from Server";
    ctx.clearRect(0, 0, 1080, 1080);
    for (var k in data)
    {
        var r = data[k];
        if (r != null) {
            //console.log(r.x + " " + r.y);
            ctx.fillStyle = "rgb("+r.r+", "+r.g+", "+r.b+")";
            ctx.fillRect(r.x - r.w / 2, r.y - r.h / 2, r.w, r.h);
        }
    }    
})
socket.on('address', function (data) {
    document.getElementById("address").innerHTML = data;
})

function onMouseMove(e)
{
    if (bMouseDown)
    {
        var box = theField.getBoundingClientRect();
        var r = new Coords();
        var X = e.clientX - box.left;
        var Y = e.clientY - box.top;
        r.x = X;
        r.y = Y;

        socket.emit("rect", r);
        document.getElementById("information").innerHTML = X + " " + Y;
    }
}
function onMouseUp(e)
{
    bMouseDown = false;
}
function onMouseLeave(e)
{
    bMouseDown = false;
}
function onMouseDown(e)
{
    bMouseDown = true;
    var box = theField.getBoundingClientRect();
    var r = new Coords();
    var X = e.clientX - box.left;
    var Y = e.clientY - box.top;
    r.x = X;
    r.y = Y;

    socket.emit("rect", r);
    document.getElementById("information").innerHTML = X + " " + Y;
}


function onTouchStart(e) {
    
    if (e.touches)
    {
        //One finger
        if (e.touches.length >= 1)
        {
            var t = e.touches[0];
            //var X = t.pageX - t.target.offsetLeft;
            //var Y = t.pageY - t.target.offsetTop;
            var box = theField.getBoundingClientRect();
            var X = t.pageX - box.left;
            var Y = t.pageY - box.top;
            var r = new Coords();
            r.x = X;
            r.y = Y;

            socket.emit("rect", r);
        }
    }
    event.preventDefault();
    

}
function onTouchMove(e){
    if (e.touches) {
        //One finger
        if (e.touches.length >= 1) {
            var t = e.touches[0];
            //var X = t.pageX - t.target.offsetLeft;
            //var Y = t.pageY - t.target.offsetTop;
            var box = theField.getBoundingClientRect();
            var X = t.pageX- box.left;
            var Y = t.pageY - box.top;
            var r = new Coords();
            r.x = X;
            r.y = Y;
            socket.emit("rect", r);
        }
    }
    e.preventDefault();

}