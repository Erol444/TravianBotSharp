import PropTypes from "prop-types";
import {
	Table,
	TableBody,
	TableCell,
	TableContainer,
	TableHead,
	TableRow,
} from "@mui/material";
import React from "react";
import { nanoid } from "nanoid";

const MUITable = ({ header, data, onClick, selected }) => {
	return (
		<>
			<TableContainer>
				<Table size="small">
					<TableHead>
						<TableRow>
							{header.map((item, index) => (
								<TableCell key={nanoid(10)} variant="head">
									{item}
								</TableCell>
							))}
						</TableRow>
					</TableHead>
					<TableBody>
						{data.map((row) => {
							const cells = [];
							Object.entries(row).forEach((item) => {
								if (item[0] === "id") return;
								cells.push(
									<TableCell key={nanoid(10)}>
										{typeof item[1] === "string"
											? item[1]
											: JSON.stringify(item[1])}
									</TableCell>
								);
							});
							return (
								<TableRow
									hover
									onClick={() => onClick(row.id)}
									selected={row.id === selected}
									key={nanoid(10)}
								>
									{cells}
								</TableRow>
							);
						})}
					</TableBody>
				</Table>
			</TableContainer>
		</>
	);
};

MUITable.propTypes = {
	data: PropTypes.array.isRequired,
	onClick: PropTypes.func,
	header: PropTypes.array.isRequired,
	selected: PropTypes.number,
};

export default MUITable;
