import PropTypes from "prop-types";
import React from "react";
import { TableCell, TableRow } from "@mui/material";

const AccountRow = ({ accounts, handler, selected }) => {
  const rows = accounts.map((account) => (
    <TableRow
      hover
      onClick={() => handler(account)}
      selected={account.id === selected}
      key={account.id}
    >
      <TableCell>{account.name}</TableCell>
      <TableCell>{account.serverUrl}</TableCell>
    </TableRow>
  ));
  return <>{rows}</>;
};

AccountRow.propTypes = {
  accounts: PropTypes.array.isRequired,
  handler: PropTypes.func.isRequired,
  selected: PropTypes.number.isRequired,
};

export default AccountRow;
