import PropTypes from "prop-types";
import {
	Table,
	TableBody,
	TableCell,
	TableContainer,
	TableHead,
	TableRow,
} from "@mui/material";
import { DragHandle, RemoveCircle } from "@mui/icons-material";

import {
	SortableContainer,
	SortableElement,
	SortableHandle,
} from "react-sortable-hoc";

import React from "react";
import { nanoid } from "nanoid";

const DragHandleIcon = SortableHandle(() => {
	return <DragHandle />;
});
const SortableRow = SortableElement(({ items, onRemove, selected }) => {
	const cells = [];
	Object.entries(items).forEach((item) => {
		if (item[0] === "id") return;
		cells.push(
			<TableCell key={nanoid(10)}>{JSON.stringify(item[1])}</TableCell>
		);
	});
	cells.push(
		<TableCell key={nanoid(10)}>
			<DragHandleIcon />
		</TableCell>
	);
	cells.push(
		<TableCell key={nanoid(10)}>
			<RemoveCircle onClick={() => onRemove(items.id)} />
		</TableCell>
	);

	return (
		<TableRow hover selected={items.id === selected} key={nanoid(10)}>
			{cells}
		</TableRow>
	);
});

const MUIDraggableTable = SortableContainer(({ header, data, onRemove }) => {
	if (data.length === 0) return <div>Empty queue</div>;
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
							<TableCell variant="head">Move</TableCell>
							<TableCell variant="head">Remove</TableCell>
						</TableRow>
					</TableHead>
					<TableBody>
						{data.map((row, index) => {
							return (
								<SortableRow
									items={row}
									index={index}
									onRemove={onRemove}
									key={nanoid(10)}
								/>
							);
						})}
					</TableBody>
				</Table>
			</TableContainer>
		</>
	);
});

MUIDraggableTable.propTypes = {
	data: PropTypes.array.isRequired,
	onRemove: PropTypes.func.isRequired,
	header: PropTypes.array.isRequired,
};

export default MUIDraggableTable;
