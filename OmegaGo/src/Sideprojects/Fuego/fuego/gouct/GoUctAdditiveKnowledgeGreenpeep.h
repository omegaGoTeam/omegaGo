//----------------------------------------------------------------------------
/** @file GoUctAdditiveKnowledgeGreenpeep.h */
//----------------------------------------------------------------------------

#ifndef GOUCT_ADDITIVEKNOWLEDGEGREENPEEP_H
#define GOUCT_ADDITIVEKNOWLEDGEGREENPEEP_H

#include "GoUctAdditiveKnowledge.h"
#include "GoUctPlayoutPolicy.h"
#include <boost/static_assert.hpp>


/* max 26-bit: 16-bit 8-neighbor core, 8-bit liberty & 2-away extension, 
	1 bit "ko exists", 1 bit defensive move */
const int NUMPATTERNS9X9 = 1<<26;

/* 24-bit: 16-bit 8-neighbor core, 8-bit liberty & 2-away extension */
const int NUMPATTERNS19X19 = 1<<24;

//----------------------------------------------------------------------------

class GoUctAdditiveKnowledgeParamGreenpeep: public GoUctAdditiveKnowledgeParam
{
private:
public:
    GoUctAdditiveKnowledgeParamGreenpeep();

    unsigned short m_predictor9x9[NUMPATTERNS9X9];
    
    unsigned short m_predictor19x19[NUMPATTERNS19X19];
};

/** Use Greenpeep-style pattern values to make predictions. */
class GoUctAdditiveKnowledgeGreenpeep : public GoUctAdditiveKnowledge
{
public:
    GoUctAdditiveKnowledgeGreenpeep(const GoBoard& bd,
				 const GoUctAdditiveKnowledgeParamGreenpeep& param);

    /** The minimum value allowed by this predictor */
    SgUctValue Minimum() const;

    bool ProbabilityBased() const;

    void ProcessPosition(std::vector<SgUctMoveInfo>& moves);

    /** Print a pattern given its pattern code. 
    	3 typical examples:

        Example 1: the all empty pattern. @b shows its black turn (all patterns
        are black turn). Because the four neighbor points of @b are empty, the
        pattern shows the 4 points one step further in the four directions.
        They are all empty in this example.
                [  ]
            [  ][  ][  ]
        [  ][  ][@b][  ][  ]
            [  ][  ][  ]
                [  ]

        Example 2: this pattern has only 12 points.
        Since b3, west of @b, is taken by black, the extra 2 bits are now 
        used to encode the number of liberties (3) of this black block.

        [##], two steps to the north, encodes the border.
            [##]
        [  ][  ][b ]
        [b3][@b][  ][b ]
        [b ][  ][b ]
            [w ]

        Example 3: an edge pattern. It is also a capture since w1 is a
        white block with 1 liberty.
            [##][b1][b ]
        [##][##][@b][w1]
            [##][  ][b ]
                [  ]
    */
    static void PrintContext(unsigned int context, std::ostream& o);

    /** The scaling factor for this predictor */
    SgUctValue Scale() const;

private:
    const GoUctAdditiveKnowledgeParamGreenpeep& m_param;

    unsigned int m_contexts[SG_MAX_ONBOARD + 1];
};

//----------------------------------------------------------------------------

inline SgUctValue GoUctAdditiveKnowledgeGreenpeep::Minimum() const
{
	return ProbabilityBased() ? 0.0001f : 0.05f;
}

inline bool GoUctAdditiveKnowledgeGreenpeep::ProbabilityBased() const
{
	return Board().Size() >= 15;
}

inline SgUctValue GoUctAdditiveKnowledgeGreenpeep::Scale() const
{
	return 0.03f;
}

//----------------------------------------------------------------------------

#endif // GOUCT_ADDITIVEKNOWLEDGEGREENPEEP_H
