require("dotenv").config();

const jsonServer = require("json-server");
const server = jsonServer.create();
const router = jsonServer.router("db.json");
const middlewares = jsonServer.defaults();

server.use(middlewares);
server.use(router);
router.render = (req, res) => {
	if (req.path.includes("/accesses/") && req.path !== "/accesses/") {
		res.jsonp(res.locals.data.accesses);
	} else if (req.path.includes("/status/") && req.path !== "/status/") {
		res.jsonp(res.locals.data.status);
	} else {
		res.jsonp(res.locals.data);
	}
};
const port = process.env.PORT_API || 3001;
server.listen(port, () => {
	console.log(`JSON Server is running at ${port}`);
});
