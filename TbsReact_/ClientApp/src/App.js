import React, { useState, useEffect } from 'react';
import  Layout  from './components/Layout';
import './custom.css'

const App = () => {
  const [selected, setSelected] = useState(0);
  useEffect(() => {
    console.log(selected);
  }, [selected])

  return (
    <Layout selected={selected} setSelected={setSelected}>
    </Layout>
  );

}

export default App;