import React from 'react';
import NavMenu from './NavMenu';

//debug view
import Debug from './Views/Debug';
import LogBoard from './Views/Debug/LogBoard'
import TaskTable from './Views/Debug/TaskTable';

const Layout = ({ selected, setSelected, isConnect }) => {
  return (
    <>
      <NavMenu selected={selected} setSelected={setSelected} />
      <div style={{margin: "1%"}}>
      <Debug 
        taskTable={<TaskTable selected={selected} isConnect={isConnect}/>}
        logBoard={<LogBoard selected={selected} isConnect={isConnect}/>}          
        />
        </div>
    </>
  );

}

export default Layout;