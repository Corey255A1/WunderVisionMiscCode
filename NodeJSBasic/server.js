'use strict';
//var http = require('http');
var http = require('http').createServer(handler) //create the server here so that it can be passed into socket.io
var fs = require('fs');
var io = require('socket.io')(http)//pass in the http server here
var port = process.env.PORT || 1337;


http.listen(port); //start listening on the port

var validFiles = [
    "/clientscripts.js",
    "/index.html",
    "/node_modules/socket.io-client/dist/socket.io.js",
    "/css/bootstrap.css",
    "/css/bootstrap-grid.css",
    "/css/bootstrap-reboot.css",
    "/css/mystyles.css"
]


//When using Node.js you have to make sure that you are serving all of the files
//and their correct types.
function handler(req, res) {
    var file = 'index.html';
    if (req.url != '/' && validFiles.indexOf(req.url.toLowerCase()) > -1)
    {
        file = req.url;
    }
    fs.readFile('./' + file, function (err, data) {
        if (!err)
        {
            var dotoffset = file.lastIndexOf('.');
            var mimetype = dotoffset == -1
                ? 'text/plain'
                : {
                    '.html': 'text/html',
                    '.ico': 'image/x-icon',
                    '.jpg': 'image/jpeg',
                    '.png': 'image/png',
                    '.gif': 'image/gif',
                    '.css': 'text/css',
                    '.js': 'text/javascript'
                }[file.substr(dotoffset)];
            res.writeHead(200, { 'Content-Type': mimetype });
            res.write(data);
            res.end();
        }

    });

}

var MaxRects = 100;
var PersistentRectangleMap = new Array(1080);
var CurrentColor = 1; //black=1 blue=2 red=3 green=4
function GetCurrentColor()
{
    switch(CurrentColor) {
        case 1:
            return {
                r:0,
                g:0,
                b:0
            };
        case 2:
            return {
                r:0,
                g:0,
                b:255
            };
        case 3:
            return {
                r:255,
                g:0,
                b:0
            };
        case 4:
            return {
                r:0,
                g:255,
                b:0
            };
    }
}
function Shape(_x, _y, _w, _h, _r, _g, _b)
{
    return {
        x: _x,
        y: _y,
        w: _w,
        h: _h,
        r: _r,
        g: _g,
        b: _b
    }
}

for (var i = 0; i < 1080; i++)
{
    PersistentRectangleMap[i] = new Array(1080);
}
io.sockets.on('connection', function (socket) {
    var thisClientIP = socket.handshake.address;
    socket.emit('color', CurrentColor);
    socket.emit('address', thisClientIP);

    socket.on('blackChk', function (data) {
        if (data == 1) CurrentColor = 1;
        socket.emit('color', CurrentColor); //Send to Client 
        socket.broadcast.emit('color', CurrentColor); //Alert the other clients of the change
    });
    socket.on('blueChk', function (data) {
        if (data == 1) CurrentColor = 2;
        socket.emit('color', CurrentColor); //Send to Client
        socket.broadcast.emit('color', CurrentColor); //Alert the other clients of the change
    });
    socket.on('redChk', function (data) {
        if (data == 1) CurrentColor = 3;
        socket.emit('color', CurrentColor); //Send to Client
        socket.broadcast.emit('color', CurrentColor); //Alert the other clients of the change
    });
    socket.on('greenChk', function (data) {
        if (data == 1) CurrentColor = 4;
        socket.emit('color', CurrentColor); //Send to Client
        socket.broadcast.emit('color', CurrentColor); //Alert the other clients of the change
    });
    socket.on('info', function (data) {
        console.log(data);
        socket.broadcast.emit('info', data);
    })

    socket.on('refresh', function (data)
    {
        if (data == 'rectlist') {
            var ListToSend = []
            for (var x = 0; x < PersistentRectangleMap.length; x++) {
                for (var y = 0; y < PersistentRectangleMap[x].length; y++) {
                    var r = PersistentRectangleMap[x][y];
                    if (r != null) {
                        ListToSend.push(r);
                    }
               }
            }
            socket.emit('rectlistdata', ListToSend);
        }
    })
    var Size = 4;
    socket.on('rect', function (data) {
        //console.log(data);
        var c = GetCurrentColor();       
        data.x = (Math.trunc(data.x/Size)*Size);
        data.y = (Math.trunc(data.y / Size) * Size);
        var shape = new Shape(data.x, data.y, Size, Size, c.r, c.g, c.b);
        PersistentRectangleMap[data.x][data.y] = shape;

        socket.emit('rect', shape);
        socket.broadcast.emit('rect', shape);        
    })
});

//the original way I saw how to do this on the W3Schools page
//http.createServer(function (req, res) {
//    fs.readFile('index.html', function (err, data) {
//        res.writeHead(200, { 'Content-Type': 'text/html' });
//        res.write(data)
//        res.end();
//    })

//}).listen(port);


