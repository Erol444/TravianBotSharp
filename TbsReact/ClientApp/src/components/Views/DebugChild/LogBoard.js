import React, { useEffect, useState } from "react";
import PropTypes from "prop-types";
import { Typography, TextareaAutosize } from "@mui/material";

import { getLogData } from "../../../api/Debug";
import { signalRConnection } from "../../../realtime/connection";
const LogBoard = ({ selected, isConnect }) => {
  const [value, setValue] = useState();

  useEffect(() => {
    if (isConnect === true) {
      signalRConnection.on("logger", (data) => {
        setValue((prev) => `${data}${prev}`);
      });

      return () => {
        signalRConnection.off("logger");
      };
    }
  }, [isConnect]);

  useEffect(() => {
    if (selected !== -1) {
      const getData = async () => {
        const data = await getLogData(selected);
        setValue(data);
      };
      getData();
    }
  }, [selected]);
  return (
    <>
      <Typography variant="h6" noWrap>
        Log
      </Typography>
      <TextareaAutosize
        style={{ width: "100%" }}
        maxRows={20}
        minRows={20}
        value={value}
        readOnly={true}
        defaultValue="Nothing here"
      />
    </>
  );
};
LogBoard.propTypes = {
  selected: PropTypes.number.isRequired,
  isConnect: PropTypes.bool.isRequired,
};
export default LogBoard;
