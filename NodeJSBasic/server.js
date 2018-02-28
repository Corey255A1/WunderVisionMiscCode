'use strict';
//var http = require('http');
var http = require('http').createServer(handler) //create the server here so that it can be passed into socket.io
var fs = require('fs');
var io = require('socket.io')(http)//pass in the http server here
var port = process.env.PORT || 1337;


http.listen(port); //start listening on the port

//When using Node.js you have to make sure that you are serving all of the files
//and their correct types.
function handler(req, res) {
    var file = 'index.html';
    if (req.url != '/')
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

var PersistentCheckBoxState = 1;

io.sockets.on('connection', function (socket) {
    socket.emit('testCtrl',PersistentCheckBoxState)
    socket.on('testCtrl', function (data) {
        PersistentCheckBoxState = data;
        console.log(PersistentCheckBoxState);        
        socket.broadcast.emit('testCtrl', PersistentCheckBoxState); //Alert the other clients of the change
    });
});

//the original way I saw how to do this on the W3Schools page
//http.createServer(function (req, res) {
//    fs.readFile('index.html', function (err, data) {
//        res.writeHead(200, { 'Content-Type': 'text/html' });
//        res.write(data)
//        res.end();
//    })

//}).listen(port);


