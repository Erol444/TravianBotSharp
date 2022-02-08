require("dotenv").config();

const jsonServer = require("json-server");
const server = jsonServer.create();
const router = jsonServer.router("db.json");
const middlewares = jsonServer.defaults();

server.use(middlewares);

router.render = (req, res) => {
	if (req.path.includes("/accesses/") && req.path !== "/accesses/") {
		res.jsonp(res.locals.data.accesses);
	} else if (req.path.includes("/status/") && req.path !== "/status/") {
		res.jsonp(res.locals.data.status);
	} else if (req.path.includes("/villages/") && req.path !== "/villages/") {
		res.jsonp(res.locals.data.villages);
	} else if (req.path.includes("/task/") && req.path !== "/task/") {
		res.jsonp(res.locals.data.task);
	} else if (req.path.includes("/log/") && req.path !== "/log/") {
		res.jsonp(res.locals.data.log);
	} else {
		res.jsonp(res.locals.data);
	}
};
server.use("/api", router);

const port = process.env.PORT_API || 3001;
server.listen(port, () => {
	console.log(`JSON Server is running at ${port}`);
});
