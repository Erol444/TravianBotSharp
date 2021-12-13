import PropTypes from "prop-types";
import React from "react";

// header
import NavMenu from "./NavMenu";

// Router
import {
  BrowserRouter as Router,
  Switch,
  Route,
  Redirect,
} from "react-router-dom";

// info view
import Info from "./Views/Info";

// debug view
import Debug from "./Views/Debug";
import LogBoard from "./Views/Debug/LogBoard";
import TaskTable from "./Views/Debug/TaskTable";

const Layout = ({ selected, setSelected, isConnect }) => {
  return (
    <Router>
      <NavMenu selected={selected} setSelected={setSelected} />
      <div style={{ margin: "1%" }}>
        <Switch>
          <Route path={"/debug"}>
            <Debug
              taskTable={
                <TaskTable selected={selected} isConnect={isConnect} />
              }
              logBoard={<LogBoard selected={selected} isConnect={isConnect} />}
            />
          </Route>
          <Route path={"/info"}>
            <Info />
          </Route>
          <Route path="*">
            <Redirect to="/info" />
          </Route>
        </Switch>
      </div>
    </Router>
  );
};

Layout.propTypes = {
  isConnect: PropTypes.bool.isRequired,
  selected: PropTypes.number.isRequired,
  setSelected: PropTypes.func.isRequired,
};

export default Layout;
